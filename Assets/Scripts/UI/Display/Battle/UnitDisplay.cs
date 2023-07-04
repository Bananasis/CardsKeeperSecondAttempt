using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using Utils;
using Zenject;

namespace Logic
{
    public abstract class UnitDisplay<T, M> : MonoPoolable<T> where T : UnitDisplay<T, M> where M : UnitModel
    {
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private Color damaged = Color.red;
        [SerializeField] private Color healed = Color.green;
        [SerializeField] private UnitHpbar _hpbar;
        [Inject] protected FightManager _fightManager;
        [Inject] protected VFXHolder _vfxHolder;
        private Tween _color;
        private Tween _position;
        protected readonly ConnectionManager cm = new();

        public override void Dispose()
        {
            _position?.Kill();
            cm.Dispose();
            base.Dispose();
        }

        public virtual void UpdateUnit(M unit)
        {
            _color?.Kill();
            if (unit.unitData.spawnVfx != null)
                _vfxHolder.Play(unit.unitData.spawnVfx, transform, 0.3f);
            _hpbar.SetMaxHp(unit.unitData.hp, unit.unitData.armor);
            icon.color = Color.white;
            transform.position = unit.positionCell.val.vector;
            unit.positionCell.Subscribe((pos) =>
            {
                _position?.Kill();
                _position = transform.DOMove(pos.vector, 0.1f / unit.unitData.speed);
            }, cm);
            unit.attack.Subscribe((tuple) =>
            {
                var (rotation, pos, attackVFX, impactVFX) = tuple;
                _position?.Kill();
                Transform transform1;
                _position = (transform1 = transform)
                    .DOMove(unit.position.vector + (pos != unit.position ? rotation.GetVector() : Vector2.zero) / 2,
                        0.2f / unit.unitData.speed)
                    .SetLoops(2, LoopType.Yoyo);
                if (unit.unitData.attackVFX != null && pos != unit.position)
                    _vfxHolder.Play(attackVFX, transform1, 0.3f,
                        !unit.unitData.attackVFX.rotatable
                            ? Quaternion.identity
                            : RotationUtils.fullRotationsQuaternion[(int) rotation]);
                if (unit.unitData.impactVFX != null)
                    _vfxHolder.Play(impactVFX, transform.parent, 0.3f,
                        !unit.unitData.impactVFX.rotatable
                            ? Quaternion.identity
                            : RotationUtils.rotationsQuaternion[(int) rotation],
                        pos.vector);
            }, cm);
            _fightManager.battle.Subscribe((b) =>
            {
                if (b) return;
                Die();
            }, cm, preInvoke: false);
            hpChange = unit.hp.TrackChange();
            cm.Add(hpChange);
            hpChange.Subscribe((hp) =>
            {
                var (newHp, oldHp) = hp;
                _hpbar.SetHp(newHp);
                if (newHp <= 0)
                {
                    Die(true);
                    return;
                }

                _color?.Kill();
                icon.color = Color.white;
                _color = icon.DOColor(newHp > oldHp ? healed : damaged, 0.2f).SetLoops(2, LoopType.Yoyo);
            }, cm, preInvoke: false);
            icon.sprite = unit.unitData.sprite;
        }

        public ChangeCell<int> hpChange { get; set; }

        public void Die(bool killed = false)
        {
            if (killed)
            {
                _color.Kill();
                _color = icon.DOColor(damaged, 0.5f);
                _color.onComplete = () =>
                {
                    _color = icon.DOFade(0, 0.5f);
                    _color.onComplete = Dispose;
                };
                return;
            }

            _color.Kill();
            _color = icon.DOFade(0, 0.5f);
            _color.onComplete = Dispose;
        }
    }
}
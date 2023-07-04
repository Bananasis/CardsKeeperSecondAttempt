using DefaultNamespace;
using DG.Tweening;
using Logic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;


public class EnemyUnit : UnitDisplay<EnemyUnit, EnemyModel>
{
    [SerializeField] private SpriteRenderer weapon;
    public override void UpdateUnit(EnemyModel enemy)
    {
        weapon.sprite = enemy.unit.weaponSprite;
        base.UpdateUnit(enemy);
        enemy.escaped.Subscribe((esc) =>
        {
            if (!esc) return;
            Die();
        }, cm);
    }
}
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    public class MobCard : UnitCard<Mob>
    {
        [SerializeField] private GameObject hpIndicator;
        [SerializeField] private GameObject DamageIndicator;
        [SerializeField] private GameObject speedIndicator;

        public void Clear()
        {
            _tagTips?.Hide();
            description.text = "";
            cardName.text = "";
            enemyImage.sprite = null;
            hpIndicator.SetActive(false);
            DamageIndicator.SetActive(false);
            speedIndicator.SetActive(false);
        }

        public override void UpdateCard(Mob unit)
        {
            hpIndicator.SetActive(true);
            DamageIndicator.SetActive(true);
            speedIndicator.SetActive(true);
            base.UpdateCard(unit);
        }
    }
}
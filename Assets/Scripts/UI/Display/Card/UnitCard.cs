using TMPro;
using UnityEngine;
using UnityEngine.UI;


public abstract class UnitCard<T> : MonoBehaviour where T : Unit
{
    [SerializeField] protected TextMeshProUGUI hp;
    [SerializeField] protected TextMeshProUGUI damage;
    [SerializeField] protected TextMeshProUGUI cardName;
    [SerializeField] protected TextMeshProUGUI description;
    [SerializeField] protected TextMeshProUGUI speed;
    [SerializeField] protected Image enemyImage;
    [SerializeField] protected TagTips _tagTips;


    public virtual void UpdateCard(T unit)
    {
        _tagTips?.UpdateTips(unit.tags);
        hp.text = $"{unit.hp}";
        damage.text = $"{unit.damage}";
        speed.text = $"{unit.speed}";
        description.text = unit.description;
        cardName.text = unit.name;
        enemyImage.sprite = unit.sprite;
    }
}
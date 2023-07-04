using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHpbar : MonoBehaviour
{
    [SerializeField] private List<Image> indicators;
    [SerializeField] private Sprite heart;
    [SerializeField] private Sprite halfHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Sprite armor;
    private int hearts;

    public void SetMaxHp(int hpPoints, int armorPoints)
    {
        indicators.ForEach((indicator) => indicator.enabled = false);
        int i = 0;
        for (; hpPoints > 1; hpPoints -= 2)
        {
            indicators[i].sprite = heart;
            indicators[i++].enabled = true;
        }

        if (hpPoints == 1)
        {
            indicators[i].sprite = halfHeart;
            indicators[i++].enabled = true;
        }

        hearts = i;

        for (; armorPoints > 0; armorPoints--)
        {
            indicators[i].sprite = armor;
            indicators[i++].enabled = true;
        }
    }

    public void SetHp(int hpPoints)
    {
        hpPoints = hpPoints < 0 ? 0 : hpPoints;
        int i = 0;
        for (; hpPoints > 1; hpPoints -= 2)
        {
            indicators[i++].sprite = heart;
        }

        if (hpPoints == 1)
            indicators[i++].sprite = halfHeart;

        for (int j = i; j < hearts; j++)
        {
            indicators[j].sprite = emptyHeart;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCard : UnitCard<Enemy>
{
    [SerializeField] private Image weaponImage;
    [SerializeField] private Image bannerImage;


    public override void UpdateCard(Enemy enemy)
    {
        base.UpdateCard(enemy);
        weaponImage.sprite = enemy.weaponSprite;
        bannerImage.sprite = enemy.banner;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Unit
{
    private readonly EnemyData data;
    private readonly WeaponData wData;
    public override Sprite sprite => data.sprite;
    public Sprite weaponSprite => wData.sprite;
    public Sprite banner => data.banner;
    public override string name => $"{data.enemyName}\n{wData.weaponName}";
    public override int power => Math.Max(data.tier, wData.tier);

    public int rob => power+1;


    public Enemy(EnemyData enemy, WeaponData weapon) : base(weapon.tags.Concat(enemy.tags).ToList(),new List<Tag>())
    {
        data = enemy;
        wData = weapon;
        damage += weapon.tier;
        speed += weapon.tier;
        hp += data.tier;
        armor += data.tier;
        damage = Mathf.Max(damage, 1);
        speed = Mathf.Max(speed, 1);
        hp = Mathf.Max(hp, 2);
        armor = Mathf.Max(armor, 0);
        
    }
}
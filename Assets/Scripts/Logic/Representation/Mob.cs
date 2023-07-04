using System.Collections.Generic;
using UnityEngine;

public class Mob : Unit
{
    public MobData data { get; }
    public override string name => data.mobName;
    public override int power => data.tier;

    public override Sprite sprite => data.sprite;


    public Mob(MobData data, List<Tag> adjacencies = default) : base(data.tags,
        adjacencies ?? new List<Tag>())
    {
        this.data = data;
        damage += data.tier;
        speed += data.tier;
        hp += data.tier;
        damage = Mathf.Max(damage, 1);
        speed = Mathf.Max(speed, 1);
        hp = Mathf.Max(hp+1, 2);
        armor = Mathf.Max(armor, 0);
    }
}
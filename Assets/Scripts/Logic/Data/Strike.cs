using System;
using System.Collections.Generic;
using System.Linq;

public struct Strike
{
    public bool blocked;
    public bool pure;
    public bool stalemate;
    public int initialDamage;
    public int positiveModifiers;
    public int negativeModifiers;
    public int block;
    public readonly UnitModel defending;
    public readonly UnitModel attacking;
    public readonly List<ISpecialModifier> specialModifiersDefending;
    public readonly List<ISpecialModifier> specialModifiersAttacking;

    public int damage
    {
        get
        {
            if (stalemate) return 1;
            if (pure) return initialDamage + positiveModifiers;
            if (blocked) return 0;
            return Math.Max(Math.Max(positiveModifiers + negativeModifiers + initialDamage, 1) - block,0);
        }
    }

    public void Attack(UnitModel attacking, UnitModel defending, ExecutorWithPreview executor)
    {


      //  var dealtDamage = damage;
        defending.hp.val -= damage;
        defending.moved = false;
        if (blocked) return;
        foreach (var m in specialModifiersAttacking)
            m.ApplyTo(defending,attacking,ref this, executor);
        foreach (var m in specialModifiersDefending)
            m.ApplyTo(attacking,defending,ref this, executor);
    }

    public Strike(UnitModel attacking, UnitModel defending, bool checkForStalemate = true)
    {
        this.attacking = attacking;
        this.defending = defending;
        specialModifiersAttacking = new List<ISpecialModifier>();
        specialModifiersDefending = new List<ISpecialModifier>();

        negativeModifiers = 0;
        positiveModifiers = 0;
        blocked = false;
        pure = false;
        stalemate = false;
        initialDamage = attacking.damage;
        block = defending.armor;

        var defendingTags = defending.tags.ToList();
        var attackingTags = attacking.tags.ToList();
        var defendingPower = defendingTags.Sum(t => t.powerBonus) + defending.unitData.power;
        var attackPower = attackingTags.Sum(t => t.powerBonus) + attacking.unitData.power;
        foreach (var t in defending.tags)
        {
            if (attackPower > defendingPower + t.power) continue;
            foreach (var mdata in t.defenceModifiers)
            {
                if (!mdata.AppliesTo(attacking.tags)) continue;
                foreach (var m in mdata.modifiers) m.Modify(ref this, mdata.self);
            }
        }

        foreach (var t in attacking.tags)
        {
            if (defendingPower > attackPower + t.power) continue;
            foreach (var mdata in t.attackModifiers)
            {
                if (!mdata.AppliesTo(defending.tags)) continue;
                foreach (var m in mdata.modifiers) m.Modify(ref this, !mdata.self);
            }
        }


        if (checkForStalemate && damage == 0)
        {
            stalemate = new Strike(defending, attacking, false).damage == 0;
        }
    }
}
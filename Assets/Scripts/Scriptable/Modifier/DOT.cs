using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DOT", menuName = "ScriptableObjects/DOT", order = 1)]
public class DOT : Modifier
{
    public bool tickDependent = true;
    public bool speedInterval;
    public bool skipFirst = true;
    public int ticks = 2;
    public float interval = 1;
    public int damage = 1;
    public float duration = 2;
    public ModifierCompositionType type = ModifierCompositionType.Renew;

    public override void Modify(ref Strike strike, bool offencive)
    {
        var mod = new AddDOTModifier()
        {
            unique = type,
            tickDependent = tickDependent,
            speedInterval = speedInterval,
            skipFirst = skipFirst,
            ticks = ticks,
            interval = interval,
            damage = damage,
            duration = duration,
        };

        if (offencive)
        {
            strike.specialModifiersAttacking.Add(mod);
            return;
        }

        strike.specialModifiersDefending.Add(mod);
    }
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AddTag", menuName = "ScriptableObjects/AddTag", order = 1)]
public class AddTag : Modifier
{
    public Tag tag;
    public float duration = 2;

    public override void Modify(ref Strike strike, bool offencive)
    {
        var mod = new AddTagModifier()
        {
            duration = duration,
            tag = tag,
            id = name
        };

        if (offencive)
        {
            strike.specialModifiersAttacking.Add(mod);
            return;
        }

        strike.specialModifiersDefending.Add(mod);
    }
}
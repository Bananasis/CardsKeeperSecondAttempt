using Logic.Data.Modifier;
using UnityEngine;

[CreateAssetMenu(fileName = "SoulEater", menuName = "ScriptableObjects/SoulEater", order = 1)]
public class SoulEater : Modifier
{
    public override void Modify(ref Strike strike, bool offencive)
    {
        if (offencive) strike.specialModifiersAttacking.Add(new SoulEaterModifier());
        else strike.specialModifiersDefending.Add(new SoulEaterModifier());
    }
}
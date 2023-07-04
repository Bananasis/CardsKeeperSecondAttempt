using Logic.Data.Modifier;
using UnityEngine;

[CreateAssetMenu(fileName = "Vampirism", menuName = "ScriptableObjects/Vampirism", order = 1)]
public class Vampirism : Modifier
{
    public override void Modify(ref Strike strike, bool offencive)
    {
        if (offencive) strike.specialModifiersAttacking.Add(new VampirismModifier());
        else strike.specialModifiersDefending.Add(new VampirismModifier());
    }
}
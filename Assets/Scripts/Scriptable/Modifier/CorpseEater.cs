using Logic.Data.Modifier;
using UnityEngine;

[CreateAssetMenu(fileName = "CorpseEater", menuName = "ScriptableObjects/CorpseEater", order = 1)]
public class CorpseEater : Modifier
{
    public override void Modify(ref Strike strike, bool offencive)
    {
        if (offencive) strike.specialModifiersAttacking.Add(new CorpseEaterModifier());
        else strike.specialModifiersDefending.Add(new CorpseEaterModifier());
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "Experienced", menuName = "ScriptableObjects/Experienced", order = 1)]
public class ExperiencedModifier : Modifier
{
    public override void Modify(ref Strike strike, bool offencive)
    {
        if (offencive)
        {
            strike.positiveModifiers += strike.defending.unitData.power;
            return;
        }

        strike.negativeModifiers -= strike.defending.unitData.power;
    }
}
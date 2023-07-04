using UnityEngine;

[CreateAssetMenu(fileName = "Modifier", menuName = "ScriptableObjects/Modifier", order = 1)]
public class Modifier : ScriptableObject
{
    public bool invulnerability;
    public bool ignoreModifiers;
    public int modifier;

    public virtual void Modify(ref Strike strike, bool offencive)
    {
        strike.blocked |= invulnerability;
        strike.pure |= ignoreModifiers;
        if (offencive)
        {
            strike.positiveModifiers += modifier;
return;
        }

        strike.negativeModifiers += modifier;

    }
}
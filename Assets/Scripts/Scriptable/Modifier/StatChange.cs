using UnityEngine;

[CreateAssetMenu(fileName = "StatChange", menuName = "ScriptableObjects/StatChange", order = 1)]
public class StatChange : Modifier
{
    
    public int damage;
    public int armor;
    public int speed;
    public float duration = 2;
    public CombineType composition = CombineType.IncreasePowerAndRenew;
    public ModifierCompositionType type = ModifierCompositionType.Renew;

    public override void Modify(ref Strike strike,bool offencive)
    {
        var mod = new StatChangeModifier()
        {
            combineRenew = composition,
            unique = type,
            duration = duration,
            armor = armor,
            damage = damage,
            speed = speed,
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
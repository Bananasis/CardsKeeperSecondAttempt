using System;

public class StatChangeModifier : SpecialModifier
{
    public CombineType combineRenew;
    
    public int armor;
    public int damage;
    public int speed;
    public float duration = 10;
    
    private int armorCumulative;
    private int damageCumulative;
    private int speedCumulative;
    private float durationLeft;


    protected override void Renew(UnitModel unit, ExecutorWithPreview executor)
    {
        durationLeft = duration;
    }

    protected override void Combine(UnitModel unit, ExecutorWithPreview executor)
    {
        switch (combineRenew)
        {
            case CombineType.IncreasePower:
                armorCumulative += armor;
                damageCumulative += damage;
                speedCumulative += speed;
                unit.armorModify += armor;
                unit.damageModify += damage;
                unit.speedModify += speed;
                break;
            case CombineType.IncreaseDuration:
                durationLeft += duration;
                break;
            case CombineType.IncreasePowerAndRenew:
                armorCumulative += armor;
                damageCumulative += damage;
                speedCumulative += speed;
                unit.armorModify += armor;
                unit.damageModify += damage;
                unit.speedModify += speed;
                Renew(unit,executor);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    protected override void Apply(UnitModel unit, ExecutorWithPreview executor)
    {
        armorCumulative = armor;
        damageCumulative = damage;
        speedCumulative = speed;
        unit.armorModify += armor;
        unit.damageModify += damage;
        unit.speedModify += speed;
        durationLeft = duration;
        void Tick()
        {
            if (expired) return;
            if (durationLeft <= 0)
            {
                expired = true;
                return;
            }

            executor.Add(durationLeft, (Tick,new PreviewData()));
            durationLeft = 0;
        }
        Tick();
        
    }

    protected override void OnExpire(UnitModel unit)
    {
        unit.armorModify -= armorCumulative;
        unit.damageModify -= damageCumulative;
        unit.speedModify -= speedCumulative;
    }

    
}

public enum CombineType
{
    IncreasePower,
    IncreaseDuration,
    IncreasePowerAndRenew,
    
}
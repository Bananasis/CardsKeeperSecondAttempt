using System;

public class AddDOTModifier : SpecialModifier
{
    public bool tickDependent;
    public bool speedInterval;
    public bool skipFirst;
    public int ticks;
    private int ticksLeft;
    public float interval;
    public int damage;
    public float duration;
    private float durationLeft;

    // public void ApplyTo(UnitModel unit, Executor executor)
    // {
    //     Action tick = null;
    //     tick = () =>
    //     {
    //         if (expired) return;
    //         var tickInterval = speedInterval ? 2f / (unit.speed + 2f) : interval;
    //         if (tickDependent)
    //         {
    //             if (ticks == 0)
    //             {
    //                 unit.RemoveModifier(this);
    //                 return;
    //             }
    //
    //             ticks--;
    //         }
    //         else
    //         {
    //             if (duration <= 0)
    //             {
    //                 unit.RemoveModifier(this);
    //                 return;
    //             }
    //
    //             duration -= tickInterval;
    //         }
    //
    //         unit.hp.val -= damage;
    //         executor.Add(tickInterval, tick);
    //     };
    //     if (!unit.AddModifier(this, unique)) return;
    //     if (skipFirst)
    //     {
    //         var tickInterval = speedInterval ? 2f / (unit.speed + 2f) : interval;
    //         duration -= tickInterval;
    //         executor.Add(tickInterval, tick);
    //         return;
    //     }
    //
    //     tick.Invoke();
    // }
    protected override void Renew(UnitModel unit, ExecutorWithPreview executor)
    {
        if (tickDependent)
        {
            ticksLeft = ticks;
        }
        else
        {
            durationLeft = duration;
        }
    }

    protected override void Combine(UnitModel unit, ExecutorWithPreview executor)
    {
        if (tickDependent)
        {
            ticksLeft += ticks;
        }
        else
        {
            durationLeft += duration;
        }
    }

    protected override void Apply(UnitModel unit, ExecutorWithPreview executor)
    {
        ticksLeft = ticks;
        durationLeft = duration;

        void Tick()
        {
            if (expired) return;
            var tickInterval = speedInterval ? unit.actionCooldown : interval;
            if (tickDependent)
            {
                if (ticksLeft == 0)
                {
                    expired = true;
                    return;
                }

                ticksLeft--;
            }
            else
            {
                if (durationLeft <= 0)
                {
                    expired = true;
                    return;
                }

                durationLeft -= tickInterval;
            }

            unit.hp.val -= damage;
            executor.Add(tickInterval, (Tick, new PreviewData()));
        }


        if (skipFirst)
        {
            var tickInterval = speedInterval ? unit.actionCooldown : interval;
            duration -= tickInterval;
            executor.Add(tickInterval, (Tick, new PreviewData()));
            return;
        }

        Tick();
    }

    protected override void OnExpire(UnitModel unit)
    {
    }
}
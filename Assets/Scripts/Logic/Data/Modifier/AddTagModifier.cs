using System.Collections.Generic;

public class AddTagModifier : SpecialModifier
{


    public Tag tag;
    public float duration = 10;
    private float durationLeft;
    
    protected override void Renew(UnitModel unit, ExecutorWithPreview executor)
    {
        durationLeft = duration;
    }

    protected override void Combine(UnitModel unit, ExecutorWithPreview executor)
    {
        durationLeft += duration;
    }

    protected override void Apply(UnitModel unit, ExecutorWithPreview executor)
    {
        unit.modifierTags.Add(tag);
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
        unit.modifierTags.Remove(tag);
    }
}
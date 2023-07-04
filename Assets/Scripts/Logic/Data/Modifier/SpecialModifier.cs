using System;

public abstract class SpecialModifier : ISpecialModifier
{
    public string id { get; set; }
    public ModifierCompositionType unique { get; set; } = ModifierCompositionType.Renew;

    public bool expired
    {
        get => _expired;
        set
        {
            if (value) Expire();
        }
    }

    private bool _expired;
    private UnitModel _unit;


    public virtual void ApplyTo(UnitModel unit, UnitModel otherUnit, ref Strike strike, ExecutorWithPreview executor)
    {
        _unit = unit;
        switch (unique)
        {
            case ModifierCompositionType.Apply:
            {
                Apply(unit, executor);
                unit.AddModifier(this);
                return;
            }
            case ModifierCompositionType.Combine:
            {
                if (!unit.TryGetModifier(this, out var mod))
                {
                    Apply(unit, executor);
                    unit.AddModifier(this);
                    return;
                }

                mod.Combine(unit, executor);
                return;
            }
            case ModifierCompositionType.Renew:
            {
                if (!unit.TryGetModifier(this, out var mod))
                {
                    Apply(unit, executor);
                    unit.AddModifier(this);
                    return;
                }

                mod.Renew(unit, executor);
                return;
            }
            case ModifierCompositionType.Skip:
            {
                if (unit.TryGetModifier(this, out var mod)) return;
                Apply(unit, executor);
                unit.AddModifier(this);
                return;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected abstract void Renew(UnitModel unit, ExecutorWithPreview executor);
    protected abstract void Combine(UnitModel unit, ExecutorWithPreview executor);
    protected abstract void Apply(UnitModel unit, ExecutorWithPreview executor);

    protected abstract void OnExpire(UnitModel unit);
    
    private  void Expire()
    {
        if(_expired) return;
        _expired = true;
        OnExpire(_unit);
        _unit.RemoveModifier(this);

    }
}
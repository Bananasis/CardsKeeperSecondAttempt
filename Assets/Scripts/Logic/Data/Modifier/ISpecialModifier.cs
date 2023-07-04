public interface ISpecialModifier
{
    public void ApplyTo(UnitModel unit, UnitModel otherUnit, ref Strike strike, ExecutorWithPreview executor);
}
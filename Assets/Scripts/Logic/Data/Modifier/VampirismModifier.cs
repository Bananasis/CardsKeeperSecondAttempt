namespace Logic.Data.Modifier
{
    public class VampirismModifier : ISpecialModifier
    {
        public void ApplyTo(UnitModel unit, UnitModel otherUnit, ref Strike strike, ExecutorWithPreview executor)
        {
            unit.hp.val += strike.damage > 0 && unit.hp.val < unit.unitData.hp ? 1 : 0;
        }
    }
}
using System;

namespace Logic.Data.Modifier
{
    public class SoulEaterModifier : ISpecialModifier
    {
        public void ApplyTo(UnitModel unit, UnitModel otherUnit, ref Strike strike, ExecutorWithPreview executor)
        {
            if (otherUnit.hp.val <= 0) unit.damageModify += 1;
        }
    }
}
using System;

namespace Logic.Data.Modifier
{
    public class CorpseEaterModifier : ISpecialModifier
    {
        public void ApplyTo(UnitModel unit, UnitModel otherUnit, ref Strike strike, ExecutorWithPreview executor)
        {
            if (otherUnit.hp.val <= 0) unit.hp.val = Math.Min(unit.hp.val + 2, unit.unitData.hp);
        }
    }
}
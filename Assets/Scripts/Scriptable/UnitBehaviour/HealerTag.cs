
using UnityEngine;

[CreateAssetMenu(fileName = "Regen", menuName = "ScriptableObjects/Regen", order = 1)]
public class RegenTag:UnitBehaviourStrategies
    {
        public override UnitModel.IAttackMetaStrategy attackMetaStrategy => new Heal();
        public override UnitModel.IMovementStrategy movementStrategy => new Fearful();

    }

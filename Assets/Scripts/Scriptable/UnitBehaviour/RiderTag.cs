
using UnityEngine;

[CreateAssetMenu(fileName = "Rider", menuName = "ScriptableObjects/Rider", order = 1)]
    public class RiderTag:UnitBehaviourStrategies
    {
        public override UnitModel.IAttackMetaStrategy attackMetaStrategy => new Rider();
        public override UnitModel.IMovementStrategy movementStrategy => new Fearful();
    }

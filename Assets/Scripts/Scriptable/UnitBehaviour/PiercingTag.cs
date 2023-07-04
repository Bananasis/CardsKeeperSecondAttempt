
using UnityEngine;

[CreateAssetMenu(fileName = "Piercing", menuName = "ScriptableObjects/Piercing", order = 1)]
    public class PiercingTag:UnitBehaviourStrategies
    {
        public override UnitModel.IAttackMetaStrategy attackMetaStrategy => new Piercing();
        public override UnitModel.IMovementStrategy movementStrategy => new Fearful();
    }

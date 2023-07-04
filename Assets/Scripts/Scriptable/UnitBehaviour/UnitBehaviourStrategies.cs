using UnityEngine;


    public abstract class UnitBehaviourStrategies:ScriptableObject
    {
        public int priority;
        public virtual UnitModel.IAttackStrategy attackStrategy => null;
        public virtual UnitModel.IMovementStrategy movementStrategy => null;
        public virtual UnitModel.IAttackMetaStrategy attackMetaStrategy => null;
        public virtual UnitModel.IMovementMetaStrategy movementMetaStrategy => null;
    }

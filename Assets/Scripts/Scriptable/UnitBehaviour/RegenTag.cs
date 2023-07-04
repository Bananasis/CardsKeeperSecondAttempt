
using UnityEngine;

[CreateAssetMenu(fileName = "Healer", menuName = "ScriptableObjects/Healer", order = 1)]
public class HealerTag:UnitBehaviourStrategies
    {
        public override UnitModel.IAttackMetaStrategy attackMetaStrategy => new Regen(); 
        public override UnitModel.IMovementStrategy movementStrategy => new Fearful();
    }

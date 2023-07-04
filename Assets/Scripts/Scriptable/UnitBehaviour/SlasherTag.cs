
using UnityEngine;

[CreateAssetMenu(fileName = "Slasher", menuName = "ScriptableObjects/Slasher", order = 1)]
    public class SlasherTag:UnitBehaviourStrategies
    {
        public override UnitModel.IAttackMetaStrategy attackMetaStrategy => new Slasher(); 
    }

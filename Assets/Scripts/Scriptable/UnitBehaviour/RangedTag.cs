
using UnityEngine;

[CreateAssetMenu(fileName = "Ranged", menuName = "ScriptableObjects/Ranged", order = 1)]
    public class RangedTag:UnitBehaviourStrategies{
        public override UnitModel.IAttackStrategy attackStrategy => new Ranged(); 
    }

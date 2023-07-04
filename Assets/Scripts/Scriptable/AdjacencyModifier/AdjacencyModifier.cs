
    
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "AdjacencyModifier", menuName = "ScriptableObjects/AdjacencyModifier", order = 1)]
    public class AdjacencyModifier: ScriptableObject
    {
        public int hp;
        public int damage;
        public int speed;
        public int armor;
        public List<Tag> addTag;
        public List<Tag> suppressTag;
        
    }

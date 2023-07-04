using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 1)]
public class EnemyData : ScriptableObject
{   
        public string enemyName;
        public Sprite sprite;
        public Sprite banner;
        public List<Tag> tags;
        public int tier;
}
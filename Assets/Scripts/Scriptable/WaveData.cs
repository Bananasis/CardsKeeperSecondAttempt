using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/Wave", order = 1)]
public class WaveData : ScriptableObject
{
    public bool treasureReward;
    public List<MobData> reward = new();
    public List<EnemyUnitData> wave = new();
    public bool entranceReward;
}
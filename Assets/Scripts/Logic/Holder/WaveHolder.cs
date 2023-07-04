using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class WaveHolder : MonoBehaviour
    {
        public int waveIndex = 0;

        [SerializeField] public List<WaveData> waves = new();
        [SerializeField] public List<MobData> mobPool = new();
        [SerializeField] public List<MobData> startHand = new();
        
        public WaveData GetWave(int levelVal)
        {
            if (levelVal >= waves.Count) return null;
            WaveData wave = waves[levelVal];
            if (levelVal > 0) mobPool= waves.Take(levelVal).SelectMany((w) =>w.reward).ToList();
            return wave;
        }

        public MobData RandomMob()
        {
            return mobPool[Random.Range(0, mobPool.Count)];
        }
    }
}
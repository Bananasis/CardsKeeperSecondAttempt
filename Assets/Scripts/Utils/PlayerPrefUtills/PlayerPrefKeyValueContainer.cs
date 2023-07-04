using System.Data;
using UnityEngine.Events;

namespace Utils
{
    public class PlayerPrefKeyValueContainer<K, V> : PlayerPrefComplexContainer<(K, V)>
    {
        public UnityEvent<V> OnValueChange = new UnityEvent<V>();

        public PlayerPrefKeyValueContainer(string name, (K, V) def = default) : base(name, def)
        {
            OnChange.AddListener((tuple) => OnValueChange.Invoke(tuple.Item2));
        }
    }
}
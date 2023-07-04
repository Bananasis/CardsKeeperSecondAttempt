using System;
using System.Collections.Generic;

namespace Utils
{
    public class PlayerPrefDictionary<K, V> : PlayerPrefContainers<K, V>
    {
        private Dictionary<K, int> keyPrefs = new Dictionary<K, int>();

        protected readonly List<PlayerPrefKeyValueContainer<K, V>> _prefContainers =
            new List<PlayerPrefKeyValueContainer<K, V>>();


        public PlayerPrefDictionary(string name, V def, IReadOnlyList<(K, V)> defValues = default) : base(name, def,
            defValues?.Count ?? 0)
        {
            SetupContainers(defValues);
            PopulatePrefDict();
        }

        public PlayerPrefDictionary(string name, V def, IReadOnlyList<K> defValues = default) : base(name, def,
            defValues?.Count ?? 0)
        {
            SetupContainers(defValues);
            PopulatePrefDict();
        }

        protected void SetupContainers(IReadOnlyList<K> defValues)
        {
            for (var i = 0; i < size; i++)
            {
                _prefContainers.Add(GetContainer(_name + $"_{i}", (defValues[i], _def)));
            }
        }

        protected void SetupContainers(IReadOnlyList<(K, V)> defValues)
        {
            for (var i = 0; i < size; i++)
            {
                _prefContainers.Add(GetContainer(_name + $"_{i}", defValues[i]));
            }
        }


        public override V this[K key]
        {
            get => _prefContainers[keyPrefs[key]].val.Item2;
            set
            {
                if (!keyPrefs.ContainsKey(key))
                {
                    Add(key, value);
                }

                int i = keyPrefs[key];
                var prefContainer = _prefContainers[keyPrefs[key]];
                if (prefContainer.val.Item2.Equals(value)) return;
                prefContainer.val = (key, value);
                OnChange.Invoke((key, value));
            }
        }

        private void Add(K key, V value)
        {
            _prefContainers.Add(GetContainer(_name + $"_{size}", (key, value)));
            keyPrefs[key] = size;
            OnAdd.Invoke((key, value));
            _size.val++;
        }

        public void Delete(K key)
        {
            int i = keyPrefs[key];
            keyPrefs.Remove(key);

            var temp = _prefContainers[size - 1];
            _prefContainers.RemoveAt(size - 1);
            OnRemove.Invoke(_prefContainers[i].val);
            _prefContainers[i] = temp;
            keyPrefs[temp.val.Item1] = i;
            _size.val--;
        }

        public override PassCell<V> GetEvent(K key)
        {
            //  return _prefContainers[keyPrefs[key]].OnValueChange;
            throw new NotImplementedException();
        }


        protected PlayerPrefKeyValueContainer<K, V> GetContainer(string name, (K, V) def)
        {
            return new PlayerPrefKeyValueContainer<K, V>(name, def);
        }

        private void PopulatePrefDict()
        {
            for (var i = 0; i < _prefContainers.Count; i++)
            {
                var (key, _) = _prefContainers[i].val;
                if (keyPrefs.ContainsKey(key))
                    throw new GameException($"Pref container {_name} already contains key {key}");
                keyPrefs[key] = i;
            }
        }
    }
}
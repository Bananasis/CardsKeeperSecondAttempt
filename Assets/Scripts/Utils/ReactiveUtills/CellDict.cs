using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public class CellDict<K, V> : CellCollection<Dictionary<K, V>, KeyValuePair<K, V>, IReadOnlyDictionary<K, V>>,
        IDictionary<K, V>, IAggregatable<V>
    {
        public ICollection<K> Keys => _val.Keys;
        public ICollection<V> Values => _val.Values;


        public CellDict(ConnectionManager masterCM = default) : base(masterCM)
        {
        }

        public override bool Contains(KeyValuePair<K, V> item)
        {
            return _val.ContainsKey(item.Key) && Equals(_val[item.Key], item.Value);
        }

        public override bool Remove(KeyValuePair<K, V> item)
        {
            if (!_val.ContainsKey(item.Key)) return false;
            if (!Equals(_val[item.Key], item.Value)) return false;
            return Remove(item.Key);
        }

        public void Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        public override void Clear()
        {
            KeyValuePair<K, V>[] temp = _val.ToArray();
            _val.Clear();

            for (var i = temp.Length - 1; i > -1; i--)
            {
                OnRemove.Invoke(temp[i]);
            }

            OnChange.Invoke(val);
        }


        public void Add(K key, V value)
        {
            _val.Add(key, value);
            OnAdd.Invoke(new KeyValuePair<K, V>(key, value));
        }

        public bool ContainsKey(K key)
        {
            return _val.ContainsKey(key);
        }

        public bool Remove(K key)
        {
            var removed = _val.Remove(key, out var value);
            if (removed) OnRemove.Invoke(new KeyValuePair<K, V>(key, value));
            return removed;
        }

        public bool TryGetValue(K key, out V value)
        {
            return _val.TryGetValue(key, out value);
        }

        public V this[K key]
        {
            get => _val[key];
            set
            {
                if (_val.TryGetValue(key, out V v))
                {
                    if (Equals(v, value)) return;
                    _val[key] = value;
                    OnChangeValue.Invoke((new KeyValuePair<K, V>(key, value), new KeyValuePair<K, V>(key, v)));
                    return;
                }

                _val[key] = value;
                OnAdd.Invoke(new KeyValuePair<K, V>(key, value));
            }
        }

        public PassCell<T> Aggregate<T>(Aggregator<V, T> aggregator)
        {
            var newAggr = new Aggregator<KeyValuePair<K, V>, T>()
            {
                OnChange = (oldAggr, tuple) =>
                    aggregator.OnChange.Invoke(oldAggr, (tuple.Item1.Value, tuple.Item2.Value)),
                OnAdd = (oldAggr, kv) => aggregator.OnAdd.Invoke(oldAggr, kv.Value),
                OnBegin = (coll) => aggregator.OnBegin.Invoke(coll.Select((kv) => kv.Value)),
                OnRemove = (oldAggr, kv) => aggregator.OnRemove.Invoke(oldAggr, kv.Value),
            };
            return Aggregate(newAggr);
        }
    }
}
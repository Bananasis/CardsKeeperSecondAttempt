using System.Collections.Generic;
using UnityEngine.Events;

namespace Utils
{
    public class CellList<V> : CellCollection<List<V>, V, IReadOnlyList<V>>, IList<V>
    {
        protected readonly UnityEvent<(int, V)> OnChangeValueWithIndex = new();

        public CellList(ConnectionManager masterCM = default) : base(masterCM)
        {
        }


        public void Add(V item)
        {
            _val.Add(item);
            OnAdd.Invoke(item);
            OnChange.Invoke(val);
        }

        public override void Clear()
        {
            V[] temp = _val.ToArray();
            _val.Clear();

            for (var i = temp.Length - 1; i > -1; i--)
            {
                OnRemove.Invoke(temp[i]);
            }

            OnChange.Invoke(val);
        }


        public override bool Remove(V item)
        {
            int index = _val.IndexOf(item);
            if (index == -1) return false;
            RemoveAt(index);
            return true;
        }

        public int IndexOf(V item)
        {
            return _val.IndexOf(item);
        }

        public void Insert(int index, V item)
        {
            _val.Insert(index, item);
            for (var i = index; i < _val.Count - 1; i++)
            {
                if (!Equals(item, _val[i + 1]))
                {
                    OnChangeValueWithIndex.Invoke((i, item));
                    OnChangeValue.Invoke((item, _val[i + 1]));
                }

                item = _val[i + 1];
            }

            OnAdd.Invoke(item);
            OnChange.Invoke(_val);
        }

        public void RemoveAt(int index)
        {
            var item = _val[index];
            _val.RemoveAt(index);
            for (var i = index; i < _val.Count; i++)
            {
                if (!Equals(item, _val[i]))
                {
                    OnChangeValue.Invoke((_val[i], item));
                }

                item = _val[i];
            }

            OnRemove.Invoke(item);
            OnChange.Invoke(_val);
        }

        public V this[int index]
        {
            get => _val[index];
            set
            {
                if (Equals(_val[index], value)) return;
                var oldValue = _val[index];
                _val[index] = value;
                OnChangeValue.Invoke((value, oldValue));
                OnChange.Invoke(val);
            }
        }
    }
}
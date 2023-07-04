using System.Collections.Generic;
using System.Linq;

namespace Utils
{
    public class CellHashset<V> : CellCollection<HashSet<V>, V, IReadOnlyCollection<V>>,
        System.Collections.Generic.ISet<V>
    {
        public CellHashset(ConnectionManager masterCM = default) : base(masterCM)
        {
        }


        void ICollection<V>.Add(V item)
        {
            if (!_val.Add(item)) return;
            OnAdd.Invoke(item);
            OnChange.Invoke(val);
        }

        public void ExceptWith(IEnumerable<V> other)
        {
            V[] temp = other.Where((item) => _val.Contains(item)).ToArray();
            foreach (var t in temp)
            {
                Remove(t);
            }
        }

        public void IntersectWith(IEnumerable<V> other)
        {
            V[] temp = other.Where((item) => !_val.Contains(item)).ToArray();
            foreach (var t in temp)
            {
                Remove(t);
            }
        }

        public bool IsProperSubsetOf(IEnumerable<V> other)
        {
            return _val.IsSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<V> other)
        {
            return _val.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<V> other)
        {
            return _val.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<V> other)
        {
            return _val.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<V> other)
        {
            return _val.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<V> other)
        {
            return _val.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<V> other)
        {
            var enumerable = other as V[] ?? other.ToArray();
            V[] temp = enumerable.Where((item) => _val.Contains(item)).ToArray();
            foreach (var t in enumerable)
            {
                Add(t);
            }

            foreach (var t in temp)
            {
                Remove(t);
            }
        }

        public void UnionWith(IEnumerable<V> other)
        {
            foreach (var t in other)
                Add(t);
        }

        public bool Add(V item)
        {
            if (!_val.Add(item)) return false;
            OnAdd.Invoke(item);
            OnChange.Invoke(val);
            return true;
        }

        public override void Clear()
        {
            V[] temp = _val.ToArray();
            foreach (var t in temp)
            {
                _val.Remove(t);
            }

            foreach (var t in temp)
            {
                OnRemove.Invoke(t);
            }

            OnChange.Invoke(val);
        }

        public override bool Remove(V item)
        {
            if (!_val.Remove(item)) return false;
            OnRemove.Invoke(item);
            OnChange.Invoke(val);
            return true;
        }
    }
}
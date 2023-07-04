using System;
using System.Collections.Generic;
using Unity.VisualScripting;


namespace Utils
{
    public enum ChangeType
    {
        Add,
        Remove,
        Change,
    }


    public struct Aggregator<V, T>
    {
        public Func<T, (V, V), T> OnChange;
        public Func<T, V, T> OnRemove;
        public Func<T, V, T> OnAdd;
        public Func<IEnumerable<V>, T> OnBegin;


        public Aggregator(Func<IEnumerable<V>, T> onBegin)
        {
            OnBegin = onBegin;
            OnChange = default;
            OnRemove = default;
            OnAdd = default;
        }
    }
}
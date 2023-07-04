using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

namespace Utils
{
    public interface IAggregatable<V>
    {
        public PassCell<T> Aggregate<T>(Aggregator<V, T> aggregator);
    }

    public abstract class CellCollection<C, V, ROC> : ICellCollection<C, V, ROC>, IAggregatable<V>
        where C : ICollection<V>, ROC, new()
        where ROC : IReadOnlyCollection<V>
    {
        protected readonly C _val = new C();
        public ROC val => _val;
        protected readonly UnityEvent<V> OnRemove = new();
        protected readonly UnityEvent<V> OnAdd = new();
        protected readonly UnityEvent<(V, V)> OnChangeValue = new();
        protected readonly UnityEvent<ROC> OnChange = new();
        protected readonly ConnectionManager cm;

        public PassCell<T> Aggregate<T>(Aggregator<V, T> aggregator)
        {
            return new AggregateCell<C, V, T, ROC>(this, cm, aggregator);
        }

        public void Dispose()
        {
            cm.Dispose();
        }


        public IDisposable Subscribe(Action<ROC> action, ConnectionManager additionalManager = default,
            bool once = false,
            bool preInvoke = true)
        {
            var connection = once
                ? OnChange.SubscribeOnce(action, cm, additionalManager)
                : OnChange.Subscribe(action, cm, additionalManager);

            if (preInvoke) action.Invoke(_val);
            return connection;
        }

        public IDisposable SubscribeAdd(Action<V> action, ConnectionManager additionalManager = default,
            bool once = false)
        {
            return once
                ? OnAdd.SubscribeOnce(action.Invoke, cm, additionalManager)
                : OnAdd.Subscribe(action.Invoke, cm, additionalManager);
        }

        public IDisposable SubscribeRemove(Action<V> action, ConnectionManager additionalManager = default,
            bool once = false)
        {
            return once
                ? OnRemove.SubscribeOnce(action, cm, additionalManager)
                : OnRemove.Subscribe(action, cm, additionalManager);
        }

        public IDisposable SubscribeChange(Action<(V, V)> action, ConnectionManager additionalManager = default,
            bool once = false)
        {
            return once
                ? OnChangeValue.SubscribeOnce(action, cm, additionalManager)
                : OnChangeValue.Subscribe(action, cm, additionalManager);
        }

        public virtual IEnumerator<V> GetEnumerator()
        {
            return _val.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<V>.Add(V item)
        {
            throw new NotImplementedException();
        }

        public abstract void Clear();

        public virtual bool Contains(V item)
        {
            return _val.Contains(item);
        }

        public void CopyTo(V[] array, int arrayIndex)
        {
            _val.ToArray().CopyTo(array, arrayIndex);
        }

        public abstract bool Remove(V item);

        public int Count => val.Count;
        public bool IsReadOnly => false;

        protected CellCollection(ConnectionManager masterCM = default)
        {
            cm = new ConnectionManager(masterCM);
        }
    }
}
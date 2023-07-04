using System;
using UnityEngine.Events;

namespace Utils
{
    public abstract class Cell<V> : ICell<V>
    {
        protected readonly ConnectionManager cm;
        protected readonly UnityEvent<V> OnChange = new();
        protected V _val;

        protected Cell(ConnectionManager masterCM = default)
        {
            cm = new ConnectionManager(masterCM);
        }


        public virtual V val
        {
            get => _val;
            set => Set(value);
        }

        protected virtual bool Set(V value)
        {
            if (Equals(_val, value)) return false;
            _val = value;
            OnChange.Invoke(value);
            return true;
        }


        public IDisposable Subscribe(Action<V> action, ConnectionManager additionalManager = default, bool once = false,
            bool preInvoke = true
        )
        {
            var connection = once
                ? OnChange.SubscribeOnce(action, cm, additionalManager)
                : OnChange.Subscribe(action, cm, additionalManager);

            if (preInvoke) action.Invoke(_val);
            return connection;
        }

        public void Dispose()
        {
            cm?.Dispose();
        }

        public ChangeCell<V> TrackChange()
        {
            return new ChangeCell<V>(this, cm);
        }

        public MapCell<V, T> Map<T>(Func<V, T> mapping)
        {
            return new MapCell<V, T>(this, cm, mapping);
        }
    }
}
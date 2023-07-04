using System;

namespace Utils
{
    public interface ICell<V> : IDisposable
    {
        public V val { get; }

        public IDisposable Subscribe(Action<V> action, ConnectionManager additionalManager = default,
            bool once = false, bool preInvoke = true);
    }
}
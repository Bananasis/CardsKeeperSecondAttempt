using System;
using System.Collections.Generic;

namespace Utils
{
    public interface ICellCollection<C, V, ROC> : ICell<ROC>, ICollection<V> where ROC : IReadOnlyCollection<V>
        where C : ICollection<V>
    {
        public IDisposable SubscribeAdd(Action<V> action, ConnectionManager additionalManager = default,
            bool once = false);

        public IDisposable SubscribeRemove(Action<V> action, ConnectionManager additionalManager = default,
            bool once = false);

        public IDisposable SubscribeChange(Action<(V, V)> action, ConnectionManager additionalManager = default,
            bool once = false);
    }
}
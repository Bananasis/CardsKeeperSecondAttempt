using System.Collections.Generic;

namespace Utils
{
    public class AggregateCell<C, V, T, ROC> : PassCell<T> where C : ICollection<V>
        where ROC : IReadOnlyCollection<V>
    {
        public AggregateCell(ICellCollection<C, V, ROC> collection, ConnectionManager masterCM,
            Aggregator<V, T> aggregator) :
            base(masterCM)
        {
            masterCM.Add(this);
            _val = aggregator.OnBegin(collection);
            if (aggregator.OnAdd != null)
                collection.SubscribeAdd((v) => val = aggregator.OnAdd(val, v), cm);
            if (aggregator.OnRemove != null)
                collection.SubscribeRemove((v) => val = aggregator.OnAdd(val, v), cm);
            if (aggregator.OnChange != null)
                collection.SubscribeChange(v => val = aggregator.OnChange(val, v), cm);
        }
    }
}
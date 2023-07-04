using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

namespace Utils
{
    public static class CellCollectionExtentions
    {
        public static PassCell<int> SumCell(this IAggregatable<int> cellCollection)
        {
            var aggregate = new Aggregator<int, int>
            {
                OnAdd = (oldAggr, value) => oldAggr + value,
                OnBegin = (collection) => collection.Sum(),
                OnRemove = (oldAggr, value) => oldAggr - value,
                OnChange = (oldAggr, tuple) => oldAggr + tuple.Item1 - tuple.Item2,
            };
            return cellCollection.Aggregate(aggregate);
        }

        public static PassCell<float> SumCell(this IAggregatable<float> cellCollection)
        {
            var aggregate = new Aggregator<float, float>
            {
                OnAdd = (oldAggr, value) => oldAggr + value,
                OnBegin = (collection) => collection.Sum(),
                OnRemove = (oldAggr, value) => oldAggr - value,
                OnChange = (oldAggr, tuple) => oldAggr + tuple.Item1 - tuple.Item2,
            };
            return cellCollection.Aggregate(aggregate);
        }

        public static PassCell<(int, int)> TrueFalseCount(this IAggregatable<bool> cellCollection)
        {
            var aggregate = new Aggregator<bool, (int, int)>
            {
                OnAdd = (oldAggr, value) =>
                {
                    if (value)
                    {
                        oldAggr.Item1++;
                        return oldAggr;
                    }

                    oldAggr.Item2++;
                    return oldAggr;
                },
                OnBegin = (collection) =>
                {
                    (int, int) count = (0, 0);
                    foreach (var b in collection)
                    {
                        if (b)
                        {
                            count.Item1++;
                            continue;
                        }

                        count.Item2++;
                    }

                    return count;
                },
                OnRemove = (oldAggr, value) =>
                {
                    if (value)
                    {
                        oldAggr.Item1--;
                        return oldAggr;
                    }

                    oldAggr.Item2--;
                    return oldAggr;
                },
                OnChange = (oldAggr, tuple) =>
                {
                    if (tuple.Item1)
                    {
                        oldAggr.Item1++;
                        oldAggr.Item2--;
                        return oldAggr;
                    }

                    oldAggr.Item2++;
                    oldAggr.Item1--;
                    return oldAggr;
                },
            };
            return cellCollection.Aggregate(aggregate);
        }

        public static PassCell<bool> AllCell(this IAggregatable<bool> cellCollection)
        {
            var countCell = cellCollection.TrueFalseCount();
            return countCell.Map((tuple) => tuple.Item2 == 0);
        }

        public static PassCell<bool> AnyCell(this IAggregatable<bool> cellCollection)
        {
            var countCell = cellCollection.TrueFalseCount();
            
            return countCell.Map((tuple) => tuple.Item1 > 0);
        }
    }
}
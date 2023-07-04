using System;

namespace Utils
{
    public class MapCell<V, T> : PassCell<T>
    {
        public MapCell(ICell<V> cell, ConnectionManager masterCM, Func<V, T> mapping) : base(masterCM)
        {
            masterCM.Add(this);
            cell.Subscribe(v => val = mapping.Invoke(v), cm);
        }
    }
}
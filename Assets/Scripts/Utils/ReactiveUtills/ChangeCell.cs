namespace Utils
{
    public class ChangeCell<V> : PassCell<(V, V)>
    {
        public ChangeCell(ICell<V> cell, ConnectionManager masterCM) : base(masterCM)
        {
            masterCM.Add(this);
            cm.Add(cell.Subscribe(v => val = (v, val.Item1), cm));
        }
    }
}
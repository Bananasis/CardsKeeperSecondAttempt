namespace Utils
{
    public class RootCell<V> : Cell<V>
    {
        public RootCell(ConnectionManager cm = default) : base(cm)
        {
        }

        protected RootCell()
        {
        }
    }
}
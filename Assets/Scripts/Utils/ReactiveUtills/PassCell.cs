using System.Data;

namespace Utils
{
    public abstract class PassCell<V> : Cell<V>
    {
        public new virtual V val
        {
            get => _val;
            protected set => base.Set(value);
        }

        protected override bool Set(V value)
        {
            throw new ReadOnlyException("Pass cell cannot be modified from outside!");
        }

        public PassCell(ConnectionManager cm) : base(cm)
        {
        }
    }
}
using UnityEngine.Events;

namespace Utils
{
    public abstract class PlayerPrefContainers<K, V>
    {
        protected readonly string _name;

        protected readonly V _def;
        protected readonly PlayerPrefInt _size;
        public int size => _size.val;
        public readonly UnityEvent<(K, V)> OnChange = new();
        public readonly UnityEvent<(K, V)> OnRemove = new();
        public readonly UnityEvent<(K, V)> OnAdd = new();


        protected PlayerPrefContainers(string name, V def, int defSize = 0)
        {
            _def = def;
            _name = name;
            _size = new PlayerPrefInt(_name + "_size", defSize);
        }

        public abstract V this[K i] { get; set; }
        public abstract PassCell<V> GetEvent(K i);
    }
}
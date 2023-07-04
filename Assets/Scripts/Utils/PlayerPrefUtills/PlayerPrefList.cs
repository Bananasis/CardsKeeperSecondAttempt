using System;
using System.Collections.Generic;

namespace Utils
{
    public abstract class PlayerPrefList<S> : PlayerPrefContainers<int, S>
    {
        private readonly List<PlayerPrefContainer<S>> _prefContainers = new List<PlayerPrefContainer<S>>();

        protected PlayerPrefList(string name, S def, int defSize = 0) : base(name, def, defSize)
        {
            SetupContainers();
        }

        protected PlayerPrefList(string name, S def, IReadOnlyList<S> defValues = default) : base(name, def,
            defValues?.Count ?? 0)
        {
            SetupContainers(defValues);
        }

        public override S this[int i]
        {
            get => _prefContainers[i].val;
            set
            {
                if (_prefContainers[i].val.Equals(value)) return;
                _prefContainers[i].val = value;
                OnChange.Invoke((i, value));
            }
        }

        public override PassCell<S> GetEvent(int i)
        {
            throw new NotImplementedException();
            //return _prefContainers[i];
        }

        protected void SetupContainers(IReadOnlyList<S> defValues)
        {
            for (int i = 0; i < size; i++)
            {
                _prefContainers.Add(GetContainer(_name + $"_{i}", defValues[i]));
            }
        }

        protected void SetupContainers()
        {
            for (int i = 0; i < size; i++)
            {
                _prefContainers.Add(GetContainer(_name + $"_{i}", _def));
            }
        }

        protected abstract PlayerPrefContainer<S> GetContainer(string name, S def);

        public void Add(S value)
        {
            OnAdd.Invoke((size, value));
            _size.val++;
            var cont = GetContainer(_name + $"_{size}", _def);
            cont.val = value;
            _prefContainers.Add(cont);
        }

        public void RemoveLast()
        {
            OnRemove.Invoke((size - 1, _prefContainers[size - 1].val));
            _size.val--;
            _prefContainers[size].Delete();
            _prefContainers.RemoveAt(size);
        }
    }
}
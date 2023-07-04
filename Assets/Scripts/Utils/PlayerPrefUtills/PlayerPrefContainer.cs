using UnityEngine;

namespace Utils
{
    public abstract class PlayerPrefContainer<S> : RootCell<S>
    {
        protected readonly string _name;
//        public abstract S val { get; set; }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(_name);
        }

        public PlayerPrefContainer(string name, S def = default)
        {
            _name = name;
        }
    }
}
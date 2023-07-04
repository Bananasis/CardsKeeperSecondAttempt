using UnityEngine;

namespace Utils
{
    public class PlayerPrefString : PlayerPrefContainer<string>
    {
        public override string val
        {
            get => _val;
            set
            {
                if (value == _val) return;
                _val = value;
                OnChange.Invoke(value);
                PlayerPrefs.SetString(_name, _val);
            }
        }

        public void Set(string value)
        {
            val = value;
        }

        public PlayerPrefString(string name, string def = default) : base(name)
        {
            _val = PlayerPrefs.GetString(name, def);
        }
    }
}
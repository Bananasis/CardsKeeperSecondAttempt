using UnityEngine;

namespace Utils
{
    public class PlayerPrefInt : PlayerPrefContainer<int>
    {
        public override int val
        {
            get => _val;
            set
            {
                if (value == _val) return;
                _val = value;
                OnChange.Invoke(value);
                PlayerPrefs.SetInt(_name, _val);
            }
        }

        public PlayerPrefInt(string name, int def = default) : base(name)
        {
            _val = PlayerPrefs.GetInt(name, def);
        }
    }
}
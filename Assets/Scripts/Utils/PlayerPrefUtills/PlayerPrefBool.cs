using UnityEngine;

namespace Utils
{
    public class PlayerPrefBool : PlayerPrefContainer<bool>
    {
        public override bool val
        {
            get => _val;
            set
            {
                if (value == _val) return;
                _val = value;
                OnChange.Invoke(value);
                PlayerPrefs.SetInt(_name, _val ? 1 : 0);
            }
        }

        public PlayerPrefBool(string name, bool def = default) : base(name)
        {
            _val = PlayerPrefs.GetInt(name, def ? 1 : 0) != 0;
        }
    }
}
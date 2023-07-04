using UnityEngine;

namespace Utils
{
    public class PlayerPrefFloat : PlayerPrefContainer<float>
    {
        public override float val
        {
            get => _val;
            set
            {
                if (value == _val) return;
                _val = value;
                OnChange.Invoke(value);
                PlayerPrefs.SetFloat(_name, _val);
            }
        }

        public PlayerPrefFloat(string name, float def = default) : base(name)
        {
            _val = PlayerPrefs.GetFloat(name, def);
        }
    }
}
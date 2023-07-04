using UnityEngine;

namespace Utils
{
    public class PlayerPrefComplexContainer<S> : PlayerPrefContainer<S>
    {
        private string _json;

        public override S val
        {
            get => _val;
            set
            {
                string newJson = JsonUtility.ToJson(value);
                if (newJson == _json) return;
                _val = value;
                OnChange.Invoke(value);
                _json = newJson;
                PlayerPrefs.SetString(_name, _json);
            }
        }


        public PlayerPrefComplexContainer(string name, S def = default) : base(name)
        {
            _json = JsonUtility.ToJson(def);
            _json = PlayerPrefs.GetString(name, _json);
            _val = JsonUtility.FromJson<S>(_json);
        }
    }
}
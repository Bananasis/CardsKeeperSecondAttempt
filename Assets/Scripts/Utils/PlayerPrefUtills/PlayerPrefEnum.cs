using System;
using UnityEngine;

namespace Utils
{
    public class PlayerPrefEnum<E> : PlayerPrefContainer<E> where E : unmanaged, Enum
    {
        public override E val
        {
            get => _val;
            set
            {
                unsafe
                {
                    if (value.Equals(_val)) return;
                    _val = value;
                    OnChange.Invoke(value);
                    var temp = _val;
                    PlayerPrefs.SetInt(_name, *(int*) (&temp));
                }
            }
        }

        public unsafe PlayerPrefEnum(string name, E def = default) : base(name)
        {
            int temp = PlayerPrefs.GetInt(name, *(int*) (&def));
            _val = *(E*) (&temp);
        }
    }
}
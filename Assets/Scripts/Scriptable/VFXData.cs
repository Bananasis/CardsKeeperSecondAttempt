using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "VFX", menuName = "ScriptableObjects/VFX", order = 1)]
    public class VFXData:ScriptableObject
    {
        public bool rotatable = true;
        public Quaternion defaultRotation = Quaternion.identity;
        public List<Sprite> sprites;
    }

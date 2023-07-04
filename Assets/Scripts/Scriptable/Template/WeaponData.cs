using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 1)]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite sprite;
    public List<Tag> tags;
    public int tier;
}
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "Tag", menuName = "ScriptableObjects/Tag", order = 1)]
public class Tag : ScriptableObject
{
    public string tagName;
    public string tagTitle;
    public string tagDescription;
    public List<ModifierData> defenceModifiers = new();
    public List<ModifierData> attackModifiers = new();
    public List<AdjacencyModifierData> adjacencyModifiers = new();

    public int hp;
    public int damage;
    public int speed;
    public int armor;
    public UnitBehaviourStrategies behaviourStrategies;

    public int powerBonus;
    public int power;
    public VFXData attack;
    public VFXData impact;
    public VFXData spawn;


}
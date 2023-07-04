using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Unit
{
    public int damage { get; protected set; }
    public int hp { get; protected set; }
    public int speed { get; protected set; }
    public int armor { get; protected set; }
    public abstract Sprite sprite { get; }
    public readonly List<Tag> tags;
    public abstract string name { get; }
    public string description { get; }

    public IEnumerable<UnitModel.IAttackStrategy> attackStrategies =>
        tags.Select(t => t.behaviourStrategies?.attackStrategy).Where(ats => ats != null);

    public IEnumerable<UnitModel.IAttackMetaStrategy> attackMetaStrategies =>
        tags.Select(t => t.behaviourStrategies?.attackMetaStrategy).Where(ats => ats != null);

    public IEnumerable<UnitModel.IMovementStrategy> movementStrategies =>
        tags.Select(t => t.behaviourStrategies?.movementStrategy).Where(ats => ats != null);

    public IEnumerable<UnitModel.IMovementMetaStrategy> movementMetaStrategies =>
        tags.Select(t => t.behaviourStrategies?.movementMetaStrategy).Where(ats => ats != null);

    public VFXData attackVFX { get; }
    public VFXData impactVFX { get; }
    public abstract int power { get; }
    public VFXData spawnVfx { get; }

    public List<AdjacencyModifier> GetModifiers(List<Tag> originalTags, List<Tag> adjacencies)
    {
        var selfModifiers = originalTags.SelectMany(tag => tag.adjacencyModifiers)
            .Where(am => am.self && am.AppliesTo(adjacencies));
        var otherModifiers = adjacencies.SelectMany(tag => tag.adjacencyModifiers)
            .Where(am => !am.self && am.AppliesTo(originalTags));
        return selfModifiers.Concat(otherModifiers).SelectMany(am => am.modifiers).ToList();
    }
    
    protected Unit(List<Tag> originalTags, List<Tag> adjacencies)
    {
        tags = new List<Tag>(originalTags);
        var modifiers = GetModifiers(originalTags,adjacencies);
        tags.AddRange(modifiers.SelectMany(m => m.addTag));
        foreach (var tag in modifiers.SelectMany(m => m.suppressTag))
        {
            tags.Remove(tag);
        }

        damage = tags.Sum((t) => t.damage) + 1 + modifiers.Sum(m => m.damage);
        hp = tags.Sum((t) => t.hp) + 3 + modifiers.Sum(m => m.hp);
        armor = tags.Sum((t) => t.armor) + modifiers.Sum(m => m.armor);
        speed = tags.Sum((t) => t.speed) + 1 + modifiers.Sum(m => m.speed);


        attackVFX = tags.Select(t => t.attack).FirstOrDefault(a => a != null);
        impactVFX = tags.Select(t => t.impact).FirstOrDefault(a => a != null);
        spawnVfx = tags.Select(t => t.spawn).FirstOrDefault(a => a != null);
        description = String.Join(',', tags.Where((t) => t.tagTitle != "").Select(t => $" {t.tagTitle}"));
    }
}
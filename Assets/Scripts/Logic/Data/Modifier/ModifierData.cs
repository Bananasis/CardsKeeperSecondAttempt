using System;
using System.Collections.Generic;

[Serializable]
public struct ModifierData
{
    public bool exlusive;
    public bool self;
    public List<Modifier> modifiers;
    public List<Tag> otherTags;

    public bool AppliesTo(IEnumerable<Tag> tags)
    {
        foreach (var tag in tags)
        {
            if(otherTags.Contains(tag)) return !exlusive;
        }
        return exlusive;
    }
}

[Serializable]
public struct AdjacencyModifierData
{
    public bool exlusive;
    public bool self;
    public List<AdjacencyModifier> modifiers;
    public List<Tag> otherTags;

    public bool AppliesTo(IEnumerable<Tag> tags)
    {
        foreach (var tag in tags)
        {
            if(otherTags.Contains(tag)) return !exlusive;
        }
        return exlusive;
    }
}


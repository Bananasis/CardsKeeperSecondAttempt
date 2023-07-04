using System;
using System.Collections.Generic;
using System.Linq;
using Logic;
using UnityEngine;


public class TagHolder : MonoBehaviour
{
    [SerializeField] public List<Tag> tags;
    public Dictionary<string, Tag> tagDict;

    private void Awake()
    {
        tagDict = tags.ToDictionary((t) => t.tagName, (t) => t);
    }
}
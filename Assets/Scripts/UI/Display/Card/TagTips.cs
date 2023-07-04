using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TagTips : MonoBehaviour
{
    [SerializeField] private List<TagTip> _tags = new();

    public void UpdateTips(IEnumerable<Tag> tags)
    {
        int i = 0;
        Hide();
        foreach (var unitTag in tags)
        {
            if (i == _tags.Count) break;
            if (_tags[i].UpdateTagTip(unitTag)) i++;
        }
    }

    public void Hide()
    {
        _tags.ForEach(t => t.Hide());
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOrder : MonoBehaviour
{
    [SerializeField] private List<ActionPreview> turns;
    private bool active;
    public void UpdateData(IEnumerable<PreviewData> actions)
    {
        active = true;
        turns.ForEach(t=> t.gameObject.SetActive(false));
        int i = 0;
        foreach (var previewData in actions)
        {
            if(i >= turns.Count) return;
            turns[i].UpdatePreview(previewData);
            turns[i++].gameObject.SetActive(true);
        }
    }

    public void UpdateData()
    {
        if(!active) return;
        active = false;
        turns.ForEach(t=> t.gameObject.SetActive(false));
    }
}

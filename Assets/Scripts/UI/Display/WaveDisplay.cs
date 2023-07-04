using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WaveDisplay : MonoBehaviour
{
    [SerializeField] private List<UiEnemyIcon> icons;
    private List<Enemy> data;
    [SerializeField] private Button left;
    [SerializeField] private Button right;
    private int shift;
    private int maxShift;
    private RectTransform _rect;

    public void Awake()
    {
        _rect = GetComponent<RectTransform>();
        left.onClick.AddListener(ShiftLeft);
        right.onClick.AddListener(ShiftRight);
    }

    public void UpdateWave(List<Enemy> enemies)
    {
        data = enemies;
        maxShift = Mathf.Max(enemies.Count - icons.Count, 0);
        shift = Mathf.Clamp(shift, 0, maxShift);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
        UpdateWave();
    }

    private void UpdateWave()
    {
        icons.ForEach(c => c.gameObject.SetActive(false));
        for (var i = 0; i < data.Count && i < icons.Count; i++)
        {
            icons[i].gameObject.SetActive(true);
            icons[i].UpdateIcon(data[i + shift]);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        left.gameObject.SetActive(shift > 0);
        right.gameObject.SetActive(shift < maxShift);
    }


    private void ShiftLeft()
    {
        shift = Mathf.Clamp(shift - 1, 0, maxShift);
        UpdateWave();
    }

    private void ShiftRight()
    {
        shift = Mathf.Clamp(shift + 1, 0, maxShift);
        UpdateWave();
    }
}
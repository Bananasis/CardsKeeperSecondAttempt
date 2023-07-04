using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MoneyDisplay : MonoBehaviour
{
    [Inject] private GameManager _gameManager;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private List<Slider> sliders;
    [SerializeField] private List<Image> sliderBg;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color overkillColor;
    public int treasurySize = 5;
    private Tween _text;

    private void Start()
    {
        _gameManager.money.TrackChange().Subscribe((m) =>
        {
            var (newMoney, oldMoney) = m;
            _text?.Kill();
            _text = DOTween.To(() => oldMoney, x =>
            {
                oldMoney = x;
                text.text = $"{oldMoney}";
            }, newMoney, 0.5f);
        });
        _gameManager.money.Subscribe(m => UpdateDisplay(_gameManager.treasuries.val, m));
        _gameManager.treasuries.Subscribe(t => UpdateDisplay(t, _gameManager.money.val));
    }

    public void UpdateDisplay(int treasuryNumber, int money)
    {
        sliders.ForEach(s => s.gameObject.SetActive(false));
        var excessMoney = money;
        int i;
        for (i = 0; i < treasuryNumber && i < sliders.Count; i++)
        {
            sliders[i].gameObject.SetActive(true);
            sliderBg[i].color = normalColor;
            if (excessMoney <= 0)
            {
                sliders[i].value = 0;
                continue;
            }

            sliders[i].value = Mathf.Min(excessMoney, treasurySize);
            excessMoney -= treasurySize;
        }

        while (excessMoney > 0 && i < sliders.Count)
        {
            sliders[i].gameObject.SetActive(true);
            sliderBg[i].color = overkillColor;
            sliders[i].value = Mathf.Min(excessMoney, treasurySize);
            excessMoney -= treasurySize;
            i++;
        }
    }
}
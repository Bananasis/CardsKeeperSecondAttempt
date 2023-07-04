using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class BuyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Inject] private GameManager gm;
    [Inject] private StoreMenu _storeMenu;
    [SerializeField] private Image icon;
    [SerializeField] private Color canBuy;
    [SerializeField] private Color cannotBuy;
    private bool hovered;
    private Tween _color;
    private Tween _pulsate;
    private Tween _rescale;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(_storeMenu.StartBuy);
        gm.money.Subscribe((m) =>
        {
            _color?.Kill();
            _color = icon.DOColor(m > 1 ? canBuy : cannotBuy, 0.5f);
            if (!hovered) return;
            _pulsate?.Kill();
            if (gm.money.val <= 1)
            {
                if (_rescale.IsPlaying())
                    _rescale.onComplete = () => _pulsate = icon.transform.DOScale(1, 0.7f);
                else
                    _pulsate = icon.transform.DOScale(1, 0.7f);
                return;
            }

            if (_rescale.IsPlaying())
                _rescale.onComplete = () => _pulsate = icon.transform.DOScale(1.5f, 0.7f).SetLoops(-1, LoopType.Yoyo);
            else
                _pulsate = icon.transform.DOScale(1.5f, 0.7f).SetLoops(-1, LoopType.Yoyo);
        });
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.reentered) return;
        hovered = true;
        _rescale?.Kill();
        _pulsate.Kill();
        _rescale = transform.DOScale(1.4f, 0.5f);
        if (gm.money.val > 1)
            _rescale.onComplete = () => { _pulsate = icon.transform.DOScale(1.5f, 0.7f).SetLoops(-1, LoopType.Yoyo); };
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventData.fullyExited) return;
        hovered = false;
        _rescale.Kill();
        _pulsate.Kill();
        _rescale = transform.DOScale(1, 0.5f);
        _rescale.onComplete = () => _pulsate = icon.transform.DOScale(1, 0.7f);
    }
}
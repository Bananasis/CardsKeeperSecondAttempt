using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class SellButton : CardDropZone
{
    public override float dropVanish => 0.8f;
    [SerializeField] private float upscale;
    [SerializeField] private float pulsateScale;
    [SerializeField] private Image icon;
    [SerializeField] private Color _sellableColor;
    [SerializeField] private Color _unsellableColor;
    [SerializeField] private Color _idleColor;
    [Inject] private GameManager _gameManager;
    private Tween _rescale, _pulsate, _color;


    protected override void OnPointerEnter(PointerEventData data, RoomCardUI drop)
    {
        _rescale?.Kill();
        _pulsate.Kill();
        _color.Kill();
        _color = icon.DOColor(drop.room.type == RoomType.Room ? _sellableColor : _unsellableColor, 0.5f);

        _rescale = transform.DOScale(upscale, 0.5f);
        if (drop.room.type == RoomType.Room)
            _rescale.onComplete = () =>
            {
                _pulsate = icon.transform.DOScale(pulsateScale, 0.7f).SetLoops(-1, LoopType.Yoyo);
            };
    }

    protected override void OnPointerExit(PointerEventData data, RoomCardUI drop, bool dropped)
    {
        _rescale.Kill();
        _pulsate.Kill();
        _color.Kill();
        _rescale = transform.DOScale(1, 0.5f);
        _color = icon.DOColor(_idleColor, 0.5f);
        _rescale.onComplete = () => _pulsate = icon.transform.DOScale(1, 0.7f);
        if (dropped && drop.room.type == RoomType.Room)
        {
            _gameManager.Sell(drop);
        }
    }
}
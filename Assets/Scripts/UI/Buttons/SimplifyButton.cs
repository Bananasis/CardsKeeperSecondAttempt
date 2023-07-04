using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class SimplifyButton : CardDropZone
{
    public override float dropVanish => 0.8f;
    [SerializeField] private float upscale;
    [SerializeField] private float pulsateScale;
    [SerializeField] private Image icon;
    [SerializeField] private Color _sellableColor;
    [SerializeField] private Color _unsellableColor;
    [SerializeField] private Color _idleColor;
    [Inject] private RoomHolder _roomHolder;
    [Inject] private Hand _hand;
    [Inject] private GameManager _gameManager;
    
    private Tween _rescale, _pulsate, _color;

    protected override void OnPointerEnter(PointerEventData data, RoomCardUI drop)
    {
        _rescale?.Kill();
        _pulsate.Kill();
        _color.Kill();
        _color = icon.DOColor(_roomHolder.CanBeSplit(drop.room) && _gameManager.money.val > 1 ? _sellableColor : _unsellableColor, 0.5f);

        _rescale = transform.DOScale(upscale, 0.5f);
        if (_roomHolder.CanBeSplit(drop.room) && _gameManager.money.val > 1)
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
        if (dropped && _roomHolder.CanBeSplit(drop.room) && _gameManager.money.val > 1)
        {
            _gameManager.money.val -= 1;
            var newRooms = _roomHolder.Split(drop.room);
            drop.Dust();
            newRooms.ForEach((r) =>_hand.AddCard(r,default,true));
        }
    }
}
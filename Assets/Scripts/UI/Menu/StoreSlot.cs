using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(RectTransform))]
public class StoreSlot : CardDropZone
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Color red;
    [SerializeField] private Color gold;
    [Inject] private StoreMenu _storeMenu;
    [Inject] private GameManager _gameManager;
    public override float dropVanish => 0;
    public Vector3 position => rect.position;
    private int price;
    private void Awake()
    {
        _gameManager.money.Subscribe((m) => SetColor(m > price));
    }

    private void SetColor(bool b)
    {
        priceText.outlineColor = b ? gold : red;
    }

    public void SetPrice(int price)
    {
        this.price = price;
        SetColor(  _gameManager.money.val > price);
        priceText.text = $"{price}";
    }

    public override bool TakeControl(RoomCardUI card)
    {
        return _storeMenu.CardInSlot(this,card);
    }

    protected override void OnPointerExit(PointerEventData data, RoomCardUI drop, bool dropped)
    {
        if (dropped)
        {
            return;
        }
        _storeMenu.TryBuy(this,drop);
    }
}

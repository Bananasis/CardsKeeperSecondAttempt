using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

[RequireComponent(typeof(RectTransform))]
public class CardMergeSlot : CardDropZone
{
    [SerializeField] private RectTransform rect;
    [Inject] private MergeRoomsMenu _mergeRoomsMenu;
    public override float dropVanish => 0;
    public Vector3 position => rect.position;

    public override bool TakeControl(RoomCardUI card)
    {
        return _mergeRoomsMenu.CanBeMergedBySlot(this, card);
    }

    protected override void OnPointerExit(PointerEventData data, RoomCardUI drop, bool dropped)
    {
        if (dropped)
        {
            if(_mergeRoomsMenu.CanBeMergedBySlot(this, drop))
                _mergeRoomsMenu.AddBySlot(this, drop);
            return;
        }
        _mergeRoomsMenu.ClearSlot(this,drop);
    }
}
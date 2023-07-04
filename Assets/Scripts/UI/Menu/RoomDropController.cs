using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class RoomDropController : DroppableOn<GridMono>,
    IPointerClickHandler
{
    [Inject] private GridMono gridMono;
    [SerializeField] private RoomDisplay roomDisplay;
    [Inject] private CardPreview _cardPreview;
    private DVector2 initPos;
    private Rotation _rotation;
    private GridDirection root;
    private bool properDrag;


    private void Update()
    {
        if (!properDrag || !Input.GetKeyDown(KeyCode.R)) return;
        _rotation = _rotation.Add(Rotation.Up);
        var newPos = new GridDirection(initPos , _rotation);
        gridMono.MoveRoom(roomDisplay.room, root * roomDisplay.room.position.val, newPos);
    }

    protected override void OnDrag(PointerEventData eventData, GridMono Zone)
    {
       
        initPos = new DVector2(eventData.pointerCurrentRaycast.worldPosition);
        var newPos = new GridDirection(initPos, _rotation);
        gridMono.MoveRoom(roomDisplay.room, root * roomDisplay.room.position.val, newPos);
    }

    protected override void OnBeginDrag(PointerEventData eventData, bool withDrop)
    {
        _cardPreview.Lock();
    }

    protected override void OnEndDrag(PointerEventData eventData, bool withDrop)
    {
        _cardPreview.Unlock();
    }

    protected override void OnBeginDrag(PointerEventData eventData, GridMono Zone)
    {
        gridMono.locked = true;
        properDrag = true;
        var gd = roomDisplay.room.position.val;
        initPos = new DVector2(eventData.pointerDrag.gameObject.transform.position);
        root = new GridDirection(initPos) / gd;
        _rotation = default;
    }

    protected override void OnEndDrag(PointerEventData eventData, GridMono Zone, bool dropped)
    {
        
        properDrag = false;
        gridMono.locked = false;
        if (!roomDisplay.room.onGrid.val) gridMono.ResetPositions(roomDisplay.room);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gridMono.locked) return;
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        gridMono.TryRemoveRoom(roomDisplay.room,true);
    }
}
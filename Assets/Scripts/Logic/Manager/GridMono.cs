using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Zenject;

public partial class GridMono : CardDropZone
{
    private Dictionary<RoomData, Room> _rooms = new();
    private HashSet<RoomData> selectedRooms = new();
    [Inject] GridData _gridData;
    [Inject] private RoomDisplay.MonoPool _roomPool;
    [Inject] private GameManager _gameManager;
    [Inject] private CardPreview _cardPreview;
    public Cell<bool> isConnected = new RootCell<bool>();
    public bool locked;
    public override float dropVanish => 1f;
    private Vector2 pos;
    private Rotation _rotation;
    private Room _room;

    protected override void OnPointerEnter(PointerEventData data, RoomCardUI drop)
    {
        locked = true;
        if (_room != null) throw new GameException("HoW?!?!?");
        DeselectAll();
        _rotation = Rotation.Left;
        var gridDirection = new GridDirection(new DVector2(data.pointerCurrentRaycast.worldPosition), _rotation);
        AddRoom(drop.room, gridDirection, out _room);
        _roomPool.Get(transform).UpdateRoom(_room);
        _cardPreview.Lock();
    }

    protected override void OnPointerMove(PointerEventData data, RoomCardUI drop)
    {
        var gridDirection = new GridDirection(new DVector2(data.pointerCurrentRaycast.worldPosition), _rotation);
        MoveRoom(_room, _room.position.val, gridDirection);
    }

    protected override void OnPointerExit(PointerEventData data, RoomCardUI drop, bool dropped)
    {
        _cardPreview.Unlock();
        if (!dropped || !_room.onGrid.val)
        {
            RemoveRoom(_room,false);
        }
        else
        {
            drop.Dispose();
        }

        _room = null;
        locked = false;
    }


    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.R) || _room == null) return;
        _rotation = _rotation.Add(Rotation.Up);
        var translation = new GridDirection(_room.position.val.start, _rotation);
        MoveRoom(_room, _room.position.val, translation);
    }
}
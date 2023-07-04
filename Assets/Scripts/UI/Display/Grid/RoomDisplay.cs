using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Debug = System.Diagnostics.Debug;
using Sequence = DG.Tweening.Sequence;


public class RoomDisplay : MonoPoolable<RoomDisplay>, IPointerEnterHandler, IPointerExitHandler

{
    
    [Inject] private TileDisplay.MonoPool _pool;
    [Inject] private IRecycler _recycler;
    [Inject] private CardPreview _cardPreview;
    
    private readonly ConnectionManager cm = new();
    private readonly List<TileDisplay> tiles = new();
    private Sequence _position;
    private bool hideOnDisable;
    public Room room { get; private set; }

    [Inject]
    public void Construct(TileDisplay.MonoPool pool)
    {
        _pool = pool;
    }

    public void UpdateRoom(Room room)
    {
        Clear();
        this.room = room;
     
        room.position.Subscribe((gDir) =>
        {
            _position?.Kill();
            _position = transform.DOLocalJump(gDir.start.vector, 0.1f, 1, 0.1f);
        }, cm);
        room.destroy.Subscribe((destroy) =>
        {
            if (destroy) ScheduleForDispose();
        }, cm);
        tiles.AddRange(room.tiles.Select((tile) =>
        {
            var tileD = _pool.Get(transform);
            tileD.UpdateTile(tile);
            cm.Add(tileD);
            return tileD;
        }));
        room.selected.Subscribe((selected) => tiles.ForEach(tile => tile.Select(selected, room.onGrid.val)), cm);
        room.onGrid.Subscribe((onGrid) => tiles.ForEach(tile => tile.Select(room.selected.val, onGrid)), cm);
    }

    public void Clear()
    {
        cm.Dispose();
        tiles.Clear();
        _position?.Kill();
        room = null;
    }

    private void OnDisable()
    {
        if (hideOnDisable) _cardPreview.Hide();
        hideOnDisable = false;
    }

    public override void Dispose()
    {
        Clear();
        base.Dispose();
    }

    private void ScheduleForDispose()
    {
        gameObject.SetActive(false);
        _recycler.Recycle(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _cardPreview.Show(room.data);
        hideOnDisable = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _cardPreview.Hide();
        hideOnDisable = false;
    }
}
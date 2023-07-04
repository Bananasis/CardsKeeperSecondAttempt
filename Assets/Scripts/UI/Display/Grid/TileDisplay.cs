using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class TileDisplay : MonoPoolable<TileDisplay>, IDisposable
{
    private readonly ConnectionManager cm = new();
    private readonly List<TileComponent> components = new();
    private readonly Dictionary<GridDirection, TileComponent> doors = new();
    private TileComponent floor;
    [Inject] private TileComponent.MonoPool _tileComponentPool;
    [Inject] private TileHolder _tileHolder;
    [Inject] private FightManager _fightManager;
    [SerializeField] private SpriteRenderer iconRenderer;
    
    private Tile tile { get; set; }

    [Inject]
    public void Construct(TileHolder tileHolder, TileComponent.MonoPool tileComponentPool)
    {
        _tileHolder = tileHolder;
        _tileComponentPool = tileComponentPool;
    }

    public override void Dispose()
    {
        Clear();
        base.Dispose();
    }

    private void Clear()
    {
        cm.Dispose();
        doors.Clear();
        components.Clear();
        tile = null;
    }

    public void UpdateTile(Tile tile)
    {
        Clear();
        _fightManager.battle.Subscribe((battle) => iconRenderer.enabled = !battle, cm);
        iconRenderer.sprite = tile.data.mob?.sprite;
        this.tile = tile;
        AddComponents();
        components.ForEach(c => { cm.Add(c); });
        foreach (var keyValuePair in doors)
        {
            cm.Add(keyValuePair.Value);
        }

        tile.doorsOpened.SubscribeChange((tuple) => doors[tuple.Item1.Key].UpdateComponent(tuple.Item1.Value
            ? _tileHolder.tileDecorationsDict[RoomDecorationType.DoorOpen]
            : _tileHolder.tileDecorationsDict[RoomDecorationType.DoorClosed]), cm);

        tile.position.Subscribe((gd) => { transform.localPosition = (tile.data.position * gd.rotation).vector; }, cm);
    }

    private void AddComponents()
    {
        GetWalls().ForEach((tuple) =>
        {
            var wall = _tileComponentPool.Get(transform);
            wall.UpdateComponent(_tileHolder.tileWallsDict[tuple.Item1], tile, tuple.Item2);
            components.Add(wall);
        });
        if (tile.data.decoration != RoomDecorationType.Floor)
        {
            var decoration = _tileComponentPool.Get(transform);
            decoration.UpdateComponent(_tileHolder.tileDecorationsDict[tile.data.decoration], tile);
            components.Add(decoration);
        }

        floor = _tileComponentPool.Get(transform);
        floor.UpdateComponent(_tileHolder.tileDecorationsDict[RoomDecorationType.Floor], tile);
        components.Add(floor);

        tile.data.doors.ForEach((gd) =>
        {
            var door = _tileComponentPool.Get(transform);
            door.UpdateComponent(tile.doorsOpened[gd]
                    ? _tileHolder.tileDecorationsDict[RoomDecorationType.DoorOpen]
                    : _tileHolder.tileDecorationsDict[RoomDecorationType.DoorClosed], tile,
                gd.rotation);
            doors[gd] = door;
        });
    }

    private List<(WallType, Rotation)> GetWalls()
    {
        var walls = new List<(WallType, Rotation)>();
        var sameRoom = tile.data.sameRoom;
        int wallCount = 0;
        for (var i = 1; i < sameRoom.Length; i += 2)
        {
            if (!sameRoom[i])
                wallCount++;
        }

        switch (wallCount)
        {
            case 0:
                break;
            case 1:
                var iRotation = Rotation.Left;
                for (int i = 0; i < 4; i++)
                {
                    if (sameRoom[(i * 2 + 1) % 8]) continue;
                    iRotation = (Rotation) i;
                    break;
                }

                walls.Add((WallType.IType, iRotation));
                break;
            case 2:
                if (sameRoom[1] == sameRoom[5])
                {
                    var hRotation = Rotation.Left;
                    if (sameRoom[1]) hRotation = Rotation.Up;
                    walls.Add((WallType.HType, hRotation));
                }
                else
                {
                    var lRotation = Rotation.Left;
                    for (var i = 0; i < 4; i++)
                    {
                        if (!sameRoom[(i * 2 + 3) % 8] || !sameRoom[(i * 2 + 5) % 8]) continue;
                        lRotation = (Rotation) i;
                        break;
                    }

                    walls.Add((WallType.LType, lRotation));
                }

                break;
            case 3:
                var uRotation = Rotation.Left;
                for (var i = 0; i < 4; i++)
                {
                    if (!sameRoom[(i * 2 + 3) % 8]) continue;
                    uRotation = (Rotation) i;
                    break;
                }

                walls.Add((WallType.UType, uRotation));
                break;
            case 4:
                walls.Add((WallType.Full, Rotation.Left));
                break;
        }

        for (int i = 0; i < 4; i++)
        {
            if (sameRoom[(i * 2 + 7) % 8] && sameRoom[(i * 2 + 1)] && !sameRoom[i * 2])
                walls.Add((WallType.Corner, (Rotation) i));
        }


        return walls;
    }

    public void Select(bool selected,bool onGrid)
    {
        
        floor.SetColor(!onGrid?Color.red:selected?Color.cyan:Color.white);
    }
}
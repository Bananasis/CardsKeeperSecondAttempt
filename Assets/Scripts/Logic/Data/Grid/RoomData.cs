using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Utils;

[Serializable]
public class RoomData
{
    // public class RoomDataSave
    // {
    //     public RoomType type;
    //     public List<TileData.TileDataSave> tiles;
    //
    //     RoomData Load()
    //     {
    //         
    //     }
    // }
    //
    // public RoomDataSave Save()
    // {
    //     return new RoomDataSave()
    //     {
    //         type = type,
    //         tiles = tiles.Select(t => t.Value.Save());
    //     }
    // }

    public RoomType type;
    [JsonIgnore] public IEnumerable<MobData> mobs => tiles.Values.Select(t => t.mob).Where(m => m != null);
    [JsonIgnore] public Dictionary<DVector2, TileData> tiles;

     public List<(DVector2, TileData)> tileList
     {
        get => tiles?.Select(pair => (pair.Key, pair.Value)).ToList();
        set => tiles = value.ToDictionary(t => t.Item1, t => t.Item2); 
     }

    [JsonIgnore] public List<GridDirection> doors => tiles.Values.SelectMany((tile) => tile.doors).ToList();

    [JsonIgnore]
    public (int, int, int, int) roomComplexity
    {
        get
        {
            var mb = this.mobs.Select(mob => mob.tier + 1).OrderByDescending(tier => tier).ToList();
            var roomShape = (0, 0, 0, 0);
            if (mb.Count == 0) return roomShape;
            roomShape.Item1 = mb[0];
            if (mb.Count == 1) return roomShape;
            roomShape.Item2 = mb[1];
            if (mb.Count == 2) return roomShape;
            roomShape.Item3 = mb[2];
            if (mb.Count == 3) return roomShape;
            roomShape.Item4 = mb[3];
            return roomShape;
        }
    }

    [JsonIgnore] public Sprite sprite { get; }

[JsonConstructor]
    public RoomData()
    {
    }

    public RoomData(IEnumerable<DVector2> tiles, int doorNumber, RoomType type, Vector2 iconOffset,
        List<MobData> mob = default,
        float randomDecorationChance = 0.1f, Sprite sprite = null)
    {
        this.sprite = sprite;
        this.type = type;
        var tileList = tiles.ToList();
        var possibleDirs = new List<GridDirection>();
        tileList.ForEach((pos) =>
        {
            foreach (var valueTuple in RotationUtils.rotations)
            {
                if (tileList.Contains(pos + valueTuple)) continue;
                possibleDirs.Add(new GridDirection(pos, pos + valueTuple));
            }
        });
        var selectedDoors = possibleDirs.Sample(doorNumber);
        this.tiles = tileList.ToDictionary((pos) => pos, (pos) =>
        {
            var decoration = type switch
            {
                RoomType.Treasury => RoomDecorationType.MoneyBig,
                RoomType.Entrance => RoomDecorationType.Entrance,
                RoomType.Room => TileHolder.RandomDecoration(randomDecorationChance),
                _ => throw new ArgumentOutOfRangeException()
            };

            return new TileData(pos, selectedDoors.Where((door) => pos == door.start), decoration,
                RotationUtils.fullRotations.Where((rot) => tileList.Contains(pos + rot))
                    .Select((rot) => new DVector2(rot).ToRotationFull()));
        });
        if (mob == null) return;
        if (this.tiles.Count < mob.Count) throw new GameException("not enough tiles");
        var mobTiles = this.tiles.ToList().Sample(mob.Count);
        for (var i = 0; i < mob.Count; i++)
        {
            mobTiles[i].Value.AddMob(mob[i]);
        }
    }
}
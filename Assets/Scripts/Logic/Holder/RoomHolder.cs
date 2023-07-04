using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Utils;
using Zenject;
using Random = UnityEngine.Random;


public class RoomHolder : MonoBehaviour
{
    [SerializeField] private List<EnemyData> enemyTemplates;
    [SerializeField] private List<WeaponData> weaponTemplates;
    [SerializeField] private List<MobData> mobTemplates;
    [SerializeField] private List<RoomShapes> roomShapes;
    [Inject] private TileHolder _tileHolder;
    private readonly Dictionary<((MobData, int), (MobData, int)), MobData> merges = new();
    private readonly Dictionary<string, MobData> mobTemplatesDict = new();

    private void Awake()
    {
        merges.Clear();
        foreach (var mob in mobTemplates)
        {
            mobTemplatesDict.Add(mob.name, mob);
            foreach (var mergeData in mob.crafts)
            {
                if (mergeData.second == null || mergeData.secondCount == 0)
                {
                    merges.Add(((mergeData.first, mergeData.firstCount), (null, 0)), mob);
                    continue;
                }

                merges.Add(((mergeData.first, mergeData.firstCount), (mergeData.second, mergeData.secondCount)), mob);
            }
        }

        roomShapeDict.Clear();
        foreach (var roomShape in roomShapes)
        {
            if (!roomShapeDict.ContainsKey(roomShape.complexity))
                roomShapeDict[roomShape.complexity] = new List<IReadOnlyList<DVector2>>();
            roomShapeDict[roomShape.complexity].Add(roomShape.GetRoomShape());
        }
    }

    public bool HaveMobsToUpgrade(RoomData roomData)
    {
        var mobs = roomData.mobs.ToList();
        if (mobs.Count == 0) return false;
        var mobsData = new Dictionary<MobData, int>();
        foreach (var mob in mobs)
        {
            if (mobsData.ContainsKey(mob))
            {
                mobsData[mob]++;
                continue;
            }

            mobsData[mob] = 1;
        }

        foreach (var (mobData1, value1) in mobsData)
        {
            for (var i = 1; i <= value1; i++)
            {
                (MobData, int) pair2 = (null, 0);
                var pair1 = (mobData1, i);
                if (merges.ContainsKey((pair1, pair2))) return true;
                foreach (var (key, value) in mobsData)
                {
                    for (var j = 1; j <= value; j++)
                    {
                        pair2 = (key, j);
                        if (merges.ContainsKey((pair1, pair2)))
                            return true;
                    }
                }
            }
        }

        return false;
    }

    public Dictionary<((MobData, int), (MobData, int)), MobData> GerMobsToUpgrade(RoomData roomData)
    {
        var mobs = roomData.mobs.ToList();
        var mergesOut = new Dictionary<((MobData, int), (MobData, int)), MobData>();
        if (mobs.Count == 0) return mergesOut;
        var mobData = new Dictionary<MobData, int>();
        foreach (var mob in mobs)
        {
            if (mobData.ContainsKey(mob))
            {
                mobData[mob]++;
                continue;
            }

            mobData[mob] = 1;
        }

        foreach (var (mobData1, value1) in mobData)
        {
            for (var i = 1; i <= value1; i++)
            {
                (MobData, int) pair2 = (null, 0);
                var pair1 = (mobData1, i);
                if (merges.ContainsKey((pair1, pair2))) mergesOut.Add((pair1, pair2), merges[(pair1, pair2)]);
                foreach (var (key, value) in mobData)
                {
                    for (var j = 1; j <= value; j++)
                    {
                        pair2 = (key, j);
                        if (merges.ContainsKey((pair1, pair2)))
                            mergesOut.Add((pair1, pair2), merges[(pair1, pair2)]);
                    }
                }
            }
        }

        return mergesOut;
    }

    public Enemy GetEnemy(WeaponData weapon, EnemyData enemy)
    {
        return new Enemy(enemy, weapon);
    }

    public MobData GetMobData(string mobName)
    {
        return mobTemplatesDict[mobName];
    }


    private readonly Dictionary<(int, int, int, int), List<IReadOnlyList<DVector2>>> roomShapeDict = new();

    public IReadOnlyList<DVector2> GetRoom((int, int, int, int) shape)
    {
        return roomShapeDict[shape].Random();
    }

    public RoomData GetRoom(RoomType type, MobData mobData = null)
    {
        if (type == RoomType.Room && mobData == null) throw new GameException("");

        var room = GetRoom((mobData == null ? 0 : mobData.tier + 2, 0, 0, 0));
        return type switch
        {
            RoomType.Entrance => new RoomData(room, 1, type, Vector2.zero,
                sprite: _tileHolder.tileDecorationsDict[RoomDecorationType.Entrance].sprite),
            RoomType.Treasury => new RoomData(room, 1, type, Vector2.zero,
                sprite: _tileHolder.tileDecorationsDict[RoomDecorationType.MoneyBig].sprite),
            RoomType.Room => new RoomData(room, 4, type,
                Vector2.zero, new List<MobData> {mobData}),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }


    public RoomData Merge(RoomData first, RoomData second)
    {
        if (!CanBeMerged(first, second)) throw new GameException("");
        var mobs = first.mobs.Concat(second.mobs).ToList();
        var mb = mobs.Select(mob => mob.tier + 2).OrderByDescending(tier => tier).ToList();
        var roomShape = (0, 0, 0, 0);
        if (mb.Count > 0)
            roomShape.Item1 = mb[0];
        if (mb.Count > 1)
            roomShape.Item2 = mb[1];
        if (mb.Count > 2)
            roomShape.Item3 = mb[2];
        if (mb.Count > 3)
            roomShape.Item4 = mb[3];
        return new RoomData(GetRoom(roomShape), 3 + mb.Count, RoomType.Room, default,
            first.mobs.Concat(second.mobs).ToList());
    }

    public bool CanBeMerged(RoomData first, RoomData second)
    {
        if (first.type != RoomType.Room || second.type != RoomType.Room) return false;
        var allMobs = first.mobs.Concat(second.mobs).ToList();
        if (allMobs.Count > 4) return false;
        var size = 1;
        if (allMobs.Any((m) => m.tier != 0)) size = 0;
        size += allMobs.Sum((m) => m.tier == 0 ? 1 : (m.tier + 2));
        return size <= 16;
    }

    public bool CanBeMerged(RoomData room)
    {
        if (room.type != RoomType.Room) return false;
        return room.mobs.Sum((m) => m.tier + 2) < 8;
    }

    // public RoomData Simplify(RoomData room)
    // {
    //     var newComplexity = (RoomComplexity) ((int) room.roomComplexity - 1);
    //     return new RoomData(GetRoom(newComplexity), (int) newComplexity + 3, RoomType.Room, default, newComplexity,
    //         room.mobs.ToList());
    // }

    public List<RoomData> Split(RoomData room)
    {
        return room.mobs.Select((m) => GetRoom(RoomType.Room, m)).ToList();
    }

    public bool CanBeSplit(RoomData room)
    {
        return room.roomComplexity.Item2 != 0;
    }

    // public bool CanBeSimplified(RoomData room)
    // {
    //     return
    //         true; //room.roomComplexity != RoomComplexity.Dot && room.mobs.ToList().Count < (int) room.roomComplexity;
    // }

    public RoomData Merge((((MobData, int), (MobData, int)), MobData) formula, RoomData cardRoom)
    {
        var dict = new Dictionary<MobData, int>();
        foreach (var cardRoomTile in cardRoom.tiles)
        {
            if (cardRoomTile.Value.mob == null) continue;
            var mob = cardRoomTile.Value.RemoveMob();
            if (!dict.ContainsKey(mob))
                dict[mob] = 0;
            dict[mob]++;
        }

        dict[formula.Item1.Item1.Item1] -= formula.Item1.Item1.Item2;
        if (formula.Item1.Item2.Item2 != 0)
            dict[formula.Item1.Item2.Item1] -= formula.Item1.Item2.Item2;
        dict.TryGetValue(formula.Item2, out int count1);
        dict[formula.Item2] = count1 + 1;

        int i = 0;
        var tilesToAddMobs = cardRoom.tiles.Values.ToList().Sample(dict.Sum(kv => kv.Value));
        foreach (var keyValuePair in dict)
        {
            for (int j = 0; j < keyValuePair.Value; j++)
            {
                tilesToAddMobs[i++].AddMob(keyValuePair.Key);
            }
        }

        return GetRoom(dict);
        ;
    }

    private RoomData GetRoom(Dictionary<MobData, int> mobs)
    {
        var mobsData = new List<MobData>();
        foreach (var keyValuePair in mobs)
        {
            for (int i = 0; i < keyValuePair.Value; i++)
            {
                mobsData.Add(keyValuePair.Key);
            }
        }

        var mobTierList = mobsData.Select(m => m.tier + 2).OrderByDescending(t => t).ToList();
        var roomShape = (0, 0, 0, 0);
        if (mobTierList.Count > 0)
            roomShape.Item1 = mobTierList[0];
        if (mobTierList.Count > 1)
            roomShape.Item2 = mobTierList[1];
        if (mobTierList.Count > 2)
            roomShape.Item3 = mobTierList[2];
        if (mobTierList.Count > 3)
            roomShape.Item4 = mobTierList[3];


        var room = GetRoom(roomShape);
        return new RoomData(room, 4, RoomType.Room,
            Vector2.zero, mobsData);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

public class EnemyModel : UnitModel<Enemy>
{
    public int robbed;
    private bool reachedEnd;
    public override UnitType type => UnitType.Enemy;
    public int rob => unit.rob;
    public readonly Cell<bool> escaped = new RootCell<bool>();
    private RoomData nextRoom;

    public override void Dispose()
    {
        base.Dispose();
        escaped.Dispose();
    }


    public override void Plan(RoomAccessible neighbours, List<int> random,
        MultiValueDictionary<DVector2, UnitModel> plannedMovement)
    {
        if (nextRoom == neighbours.thisRoom)
            if (reachedEnd || _treasuries.Count(t => t.Key != neighbours.thisRoom) == 0)
            {
                int closestDist = _entrances.Min((e) => _allPaths[(neighbours.thisRoom, e.Value)].Item2);
                nextRoom = _entrances.Where(e => _allPaths[(neighbours.thisRoom, e.Value)].Item2 == closestDist)
                    .ToList().Random().Value;
                if (nextRoom == neighbours.thisRoom)
                {
                    OnMove(nextRoom);
                }
            }
            else
            {
                int closestDist = _treasuries.Min((e) => _allPaths[(neighbours.thisRoom, e.Key)].Item2);
                nextRoom = _treasuries.Where(e => _allPaths[(neighbours.thisRoom, e.Key)].Item2 == closestDist)
                    .ToList().Random().Key;
            }

        GetPlan(neighbours, random, plannedMovement,
            _allPaths[(neighbours.thisRoom, nextRoom)].Item1.ToList().Random());
    }

    public override bool OnMove(RoomData roomData)
    {
        if (roomData.type == RoomType.Treasury)
        {
            if(!_treasuries.TryGetValue(roomData,out int alreadyRobbed)) return false;
            var robbedCurrent = Math.Min(5 - alreadyRobbed, rob - robbed);
            robbed += robbedCurrent;
            _treasuries[roomData] += robbedCurrent;
            if (_treasuries[roomData] == 5) _treasuries.Remove(roomData);
            if (robbed == rob) reachedEnd = true;
            return false;
        }

        if (!reachedEnd || !_entrances.ContainsValue(roomData)) return false;
        escaped.val = true;
        return true;
    }

    public override PreviewData GetPreviewData()
    {
        return new PreviewData(){sprite = unit.sprite,additionalSprite = unit.weaponSprite};
    }


    private readonly Dictionary<RoomData, int> _treasuries;
    private readonly Dictionary<(RoomData, RoomData), (HashSet<RoomData>, int)> _allPaths;
    private readonly Dictionary<DVector2, RoomData> _entrances;

    public EnemyModel(Enemy enemy, Dictionary<RoomData, int> treasuries,
        Dictionary<DVector2, RoomData> entrances,
        Dictionary<(RoomData, RoomData), (HashSet<RoomData>, int)> allPaths, DVector2 pos) : base(enemy, pos)
    {
        _movementStrategies.Add(new GoToRoom());
        _movementStrategies.Add(new WonderRandomlyNoBacktrackSameRoom());
        _movementStrategies.Add(new WonderRandomlySameRoom());
        robbed = 0;
        _allPaths = allPaths;
        _treasuries = treasuries;
        _entrances = entrances;
        int closestDist = _entrances.Min((e) => _allPaths[(entrances[pos], e.Value)].Item2);
        nextRoom = _entrances.Where(e => _allPaths[(entrances[pos], e.Value)].Item2 == closestDist)
            .ToList().Random().Value;
    }
}
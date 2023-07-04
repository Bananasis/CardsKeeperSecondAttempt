using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using Zenject;
using Random = UnityEngine.Random;


public class FightManager : MonoBehaviour
{
    [SerializeField] private float spawnRate = 1;
    private float _battleSpeed;
    [SerializeField] private float battleSpeed = 1f;
    [SerializeField] private float battleSpeedAccelerated = 2f;
    [SerializeField] private float battleSpeedVeryAccelerated = 4f;
    [SerializeField] private float battleSpeedSkipAccelerated = 20f;
    [Inject] private GridData _gridData;
    [Inject] private GameManager _gameManager;
    [Inject] private EnemyUnit.MonoPool enemyPool;
    [Inject] private MobUnit.MonoPool mobPool;
    [Inject] private TurnOrder _turnOrder;
    public UnityEvent OnLevelPassed = new();
    private readonly MultiValueDictionary<DVector2, UnitModel> positions = new();
    private readonly HashSet<UnitModel> units = new();
    private readonly ExecutorWithPreview executor = new();
    private readonly List<int> random = new() {0, 1, 2, 3, 4};
    private Dictionary<RoomData, RoomAccessible> _accessibles;
    private List<Enemy> _wave;
    public readonly Cell<bool> battle = new RootCell<bool>();
    private readonly Cell<int> enemyCount = new RootCell<int>();

    public enum BattleSpeed
    {
        Pause,
        Normal,
        Accelerated,
        VeryAccelerated,
        Skip
    }

    public void SetSpeed(BattleSpeed speed)
    {
        _battleSpeed = speed switch
        {
            BattleSpeed.Pause => 0,
            BattleSpeed.Normal => battleSpeed,
            BattleSpeed.Accelerated => battleSpeedAccelerated,
            BattleSpeed.VeryAccelerated => battleSpeedVeryAccelerated,
            BattleSpeed.Skip => battleSpeedSkipAccelerated,
            _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, null)
        };
        if (speed == BattleSpeed.Pause)
        {
            executor.TryMakeBatch(Time.fixedDeltaTime);
            _turnOrder.UpdateData(executor.GetPreview());
            return;
        }

        _turnOrder.UpdateData();
    }

    public void MakeMove()
    {
        if (_battleSpeed > 0) return;
        while (true)
        {
            if (executor.ExecuteNext(Time.fixedDeltaTime))
            {
                _turnOrder.UpdateData(executor.GetPreview());
                return;
            }
        }
    }


    private void Awake()
    {
        enemyCount.Subscribe((c) =>
        {
            if (!battle.val) return;
            if (c == 0) executor.Add(1, (() =>
            {
                Win();
                OnLevelPassed.Invoke();
            }, new PreviewData()));
        });
    }

    private void FixedUpdate()
    {
        if (!battle.val) return;
        executor.ExecuteAll(Time.fixedDeltaTime, Time.fixedDeltaTime * _battleSpeed);
    }

    private void Win()
    {
        battle.val = false;
        
        foreach (var unitModel in units)
        {
            unitModel.Dispose();
        }

        units.Clear();
        executor.Clear();
        positions.Clear();
        int treasuriesNonEmpty = treasuries.Count(kv => kv.Value == 0);
        treasuries.Clear();
        entrances.Clear();
        _accessibles.Clear();
        _accessibles = null;
        _gameManager.NextWave(treasuriesNonEmpty);
    }

    public RoomAccessible GetAccessible(DVector2 pos)
    {
        return !_gridData.GetRoom(pos, out var room) ? null : _accessibles[room];
    }

    private bool Move(UnitModel unit)
    {
        if (!positions.Remove(unit.position, unit)) throw new GameException("Unit lost its position");
        var accessible = GetAccessible(unit.position);
        if (accessible == null) throw new GameException("no room");
        unit.Plan(accessible, random, positions);
        bool swapped = false;
        foreach (var keyValuePair in unit.priority.OrderBy((kv) => -kv.Value))
        {
            if (!accessible.GetRoom(keyValuePair.Key, out _)) continue;
            var pos = positions.GetValues(keyValuePair.Key, false);
            if (pos == null)
            {
                unit.position = keyValuePair.Key;
                break;
            }

            foreach (var otherUnit in pos)
            {
                if (otherUnit == unit) continue;
                if (otherUnit.type != unit.type) continue;
                otherUnit.Plan(GetAccessible(otherUnit.position), random, positions);
                if (otherUnit.priority[unit.position] !=
                    otherUnit.priority.Max((keyVal) => keyVal.Value)) continue;
                positions.Remove(otherUnit.position, otherUnit);
                (otherUnit.position, unit.position) = (unit.position, otherUnit.position);
                positions.Add(otherUnit.position, otherUnit);
                if (!_gridData.GetRoom(otherUnit.position, out var otherUnitRoom)) throw new GameException("");
                otherUnit.Plan(GetAccessible(otherUnit.position), random, positions);
                unit.OnMove(otherUnitRoom);
                swapped = true;
                break;
            }

            if (swapped) break;
        }

        positions.Add(unit.position, unit);
        unit.Plan(GetAccessible(unit.position), random, positions);
        if (!_gridData.GetRoom(unit.position, out var room)) throw new GameException("");
        return unit.OnMove(room);
    }


    private Dictionary<DVector2, RoomData> entrances;
    private Dictionary<RoomData, int> treasuries;

    private void SpawnEnemies()
    {
        if (treasuries.Count == 0)
        {
            enemyCount.val -= _wave.Count;
            return;
        }

        foreach (var keyValuePair in entrances)
        {
            var spawnPoint = keyValuePair.Key;
            if (positions.ContainsKey(spawnPoint)) continue;
            if (_wave.Count == 0) return;
            var enemy = new EnemyModel(_wave[^1], treasuries, entrances, allPaths, spawnPoint);
            _wave.RemoveAt(_wave.Count - 1);
            positions.Add(spawnPoint, enemy);

            var monoUnit = enemyPool.Get(transform, spawnPoint.vector);
            monoUnit.UpdateUnit(enemy);
            units.Add(enemy);
            enemy.escaped.Subscribe((escaped) =>
            {
                if (!escaped) return;
                _gameManager.money.val -= enemy.robbed;
                Die(enemy);
                enemyCount.val--;
            });
            enemy.hp.Subscribe((_) =>
            {
                if (!TryDie(enemy)) return;
                enemyCount.val--;
                var killReward = enemy.rob * 8f / (2f * _gameManager.level.val + 10f);
                var reward = Mathf.FloorToInt(killReward) +
                             (Random.value < killReward - Mathf.FloorToInt(killReward) ? 1 : 0);
                _gameManager.money.val += reward;
            });
     

            executor.Add(-0.1f, (() => Act(enemy), enemy.GetPreviewData()));
        }

        if (_wave.Count > 0)
            executor.Add(spawnRate + Random.value - 0.5f, (SpawnEnemies, new PreviewData()));
    }

    private void Die(UnitModel unit)
    {
        if (!units.Remove(unit))
            throw new Exception();
        if (!positions.Remove(unit.position, unit))
            throw new Exception();
        unit.Dispose();
    }


    private bool TryDie(UnitModel unit)
    {
        if (unit.hp.val > 0) return false;
        Die(unit);
        return true;
    }

    private void SpawnMobs()
    {
        foreach (var keyValuePair in _gridData.rooms)
        {
            foreach (var valuePair in keyValuePair.Key.tiles)
            {
                if (keyValuePair.Key.type != RoomType.Room) continue;
                var spawnPoint = valuePair.Key * keyValuePair.Value;
                if (positions.ContainsKey(spawnPoint)) throw new Exception("");
                if (valuePair.Value.mob == null) continue;
                var mob = new MobModel(
                    new Mob(valuePair.Value.mob, keyValuePair.Key.mobs.SelectMany(m => m.tags).ToList()), spawnPoint,
                    keyValuePair.Key);
                positions.Add(spawnPoint, mob);
                units.Add(mob);
                var monoUnit = mobPool.Get(transform, spawnPoint.vector);
                monoUnit.UpdateUnit(mob);
                mob.hp.Subscribe((_) => TryDie(mob));
                executor.Add(0.1f, (() => Act(mob), mob.GetPreviewData()));
            }
        }
    }

    private void Act(UnitModel unit)
    {
        if (!units.Contains(unit))
            return;

        if (unit.moved)
        {
            unit.moved = false;
            executor.Add(unit.actionCooldown * 0.5f,
                (() => Act(unit), unit.GetPreviewData()));
            return;
        }

        var (canMove, actionCdMultiplier) = Fight(unit);
        if (!canMove)
        {
            executor.Add(unit.actionCooldown * actionCdMultiplier, (() => Act(unit), unit.GetPreviewData()));
            return;
        }

        if (Move(unit)) return;
        unit.moved = true;
        executor.Add(unit.actionCooldown * (actionCdMultiplier + 0.5f),
            (() => Act(unit), unit.GetPreviewData()));
    }

    private (bool, float) Fight(UnitModel unit)
    {
        random.Shuffle();
        return unit.Attack(random, GetAccessible(unit.position), positions, executor);
    }

    private Dictionary<(RoomData, RoomData), (HashSet<RoomData>, int)> allPaths;

    public void StartWave(List<Enemy> wave)
    {
        SetSpeed(BattleSpeed.Normal);
        enemyCount.val = wave.Count;
        _gridData.GetPaths(out allPaths);
        treasuries = _gridData.rooms
            .Where(r => r.Key.type == RoomType.Treasury)
            .ToDictionary((r) => r.Key, (_) => 0);
        entrances = _gridData.rooms
            .Where(r => r.Key.type == RoomType.Entrance)
            .ToDictionary((r) => r.Value.start, (r) => r.Key);
        battle.val = true;
        _wave = new List<Enemy>(wave);
        _accessibles = _gridData.GetAllAccesibles();
        SpawnMobs();
        executor.Add(1f, (SpawnEnemies, new PreviewData()));
    }
}


public enum UnitType
{
    Mob,
    Enemy
}
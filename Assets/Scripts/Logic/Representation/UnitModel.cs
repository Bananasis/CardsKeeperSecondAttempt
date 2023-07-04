using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utils;

public abstract class UnitModel<T> : UnitModel where T : Unit
{
    public readonly T unit;
    public override Unit unitData => unit;
    public override int armor => Mathf.Max(armorModify + unit.armor, -1);
    public override int damage => Mathf.Max(damageModify + unit.damage, 1);
    public override int speed => Mathf.Max(speedModify + unit.speed,-3);
    public override float actionCooldown => speed > -1 ? 3f / (3f + speed) : 1-speed/3f;
    private readonly List<SpecialModifier> modifiers = new();
    public override IEnumerable<Tag> tags => unit.tags.Concat(modifierTags);

    public override void AddModifier(SpecialModifier modifier)
    {
        modifiers.Add(modifier);
    }

    public override bool TryGetModifier(SpecialModifier modifier, out SpecialModifier mod)
    {
        mod = modifiers.FirstOrDefault(m => m.id == modifier.id);
        return mod != null;
    }

    public override void RemoveModifier(SpecialModifier modifier)
    {
        modifiers.Remove(modifier);
    }

    public override (bool, float) Attack(List<int> random, RoomAccessible neighbours,
        MultiValueDictionary<DVector2, UnitModel> positionsArr, ExecutorWithPreview executor)
    {
        foreach (var strategy in _attackMetaStrategies)
        {
            var (attacked, canMove) =
                strategy.Attack(this, random, neighbours, positionsArr, _attackStrategies, executor);
            if (attacked) return (canMove, strategy.actionMultiplier);
        }

        return (true, 0.0f);
    }


    protected readonly List<IAttackMetaStrategy> _attackMetaStrategies = new();
    protected readonly List<IAttackStrategy> _attackStrategies = new();
    protected readonly List<IMovementStrategy> _movementStrategies = new();

    protected UnitModel(T unitData, DVector2 pos)
    {
        _attackStrategies.AddRange(unitData.attackStrategies);
        _attackStrategies.Add(new Melee());
        _attackMetaStrategies.AddRange(unitData.attackMetaStrategies);
        _attackMetaStrategies.Add(new SimpleAttack());
        _attackMetaStrategies.Sort((s1, s2) => -s1.priority.CompareTo(s2.priority));
        _movementStrategies.AddRange(unitData.movementStrategies);
        unit = unitData;
        hp.val = unitData.hp;
        positionCell.val = pos;
        lastPos = pos;
    }


    protected void GetPlan(RoomAccessible neighbours, List<int> random,
        MultiValueDictionary<DVector2, UnitModel> plannedMovement, RoomData nextRoom)
    {
        priority.Clear();
        int priorityIndex = 10;

        foreach (var movementStrategy in _movementStrategies)
        {
            foreach (var i in random)
            {
                var move = position + (i == 4 ? (0, 0) : RotationUtils.rotations[i]);
                if (!neighbours.StraightLine(move, position)) continue;
                if (!neighbours.GetRoom(move, out var otherRoom)) continue;
                if (!movementStrategy.GetMove(this, otherRoom, nextRoom,
                        plannedMovement.GetValues(position, false), move, neighbours.thisRoom,
                        otherRoom))
                    continue;
                if (!priority.ContainsKey(move))
                {
                    priority[move] = random[i];
                }

                priority[move] += 2 << priorityIndex;
            }

            priorityIndex--;
        }

        if (!priority.ContainsKey(position))
        {
            priority[position] = -99;
        }

        foreach (var valueTuple in RotationUtils.rotations)
        {
            if (!priority.ContainsKey(position + valueTuple)) priority[position + valueTuple] = -99;
        }
    }

    public override DVector2 position
    {
        get => positionCell.val;
        set
        {
            if (value == positionCell.val) return;
            lastPos = positionCell.val;
            positionCell.val = value;
        }
    }


    public override bool OnMove(RoomData roomData)
    {
        return false;
    }

    public override void Dispose()
    {
        modifiers.ToList().ForEach(m => m.expired = true);
        modifiers.Clear();
        hp?.Dispose();
        positionCell?.Dispose();
    }
}

public abstract partial class UnitModel : IDisposable
{
    public abstract void Dispose();
    public abstract UnitType type { get; }
    public int speedModify = 0;
    public int damageModify = 0;
    public int armorModify = 0;
    public abstract float actionCooldown { get; }
    public abstract int armor { get; }
    public abstract int damage { get; }
    public abstract int speed { get; }
    public readonly List<Tag> modifierTags = new();
    public abstract DVector2 position { get; set; }
    public readonly Cell<DVector2> positionCell = new RootCell<DVector2>();
    public readonly RootCell<int> hp = new();
    public readonly UnityEvent<(FullRotation, DVector2, VFXData, VFXData)> attack = new();
    public readonly Dictionary<DVector2, int> priority = new();
    public DVector2 lastPos { get; protected set; }
    public abstract Unit unitData { get; }
    public abstract IEnumerable<Tag> tags { get; }
    public bool moved { get; set; }

    public abstract void Plan(RoomAccessible neighbours, List<int> random,
        MultiValueDictionary<DVector2, UnitModel> plannedMovement);

    public abstract bool OnMove(RoomData roomData);

    public abstract (bool, float) Attack(List<int> random, RoomAccessible neighbours,
        MultiValueDictionary<DVector2, UnitModel> positionsArr, ExecutorWithPreview executor);

    public abstract void AddModifier(SpecialModifier modifier);
    public abstract void RemoveModifier(SpecialModifier statChangeModifier);
    public abstract bool TryGetModifier(SpecialModifier addTagModifier, out SpecialModifier mod);
    public abstract PreviewData GetPreviewData();
}
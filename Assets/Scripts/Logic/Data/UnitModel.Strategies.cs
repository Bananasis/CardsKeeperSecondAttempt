using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public partial class UnitModel
{
    public interface IMovementStrategy
    {
        public bool GetMove(UnitModel unit, RoomData neighbour, RoomData nextRoom,
            HashSet<UnitModel> inSight, DVector2 move, RoomData thisRoom, RoomData roomInSight);

        virtual bool skippable => true;
    }

    public interface IAttackStrategy
    {
        public bool GetAttack(UnitModel unit, UnitModel otherUnit);
    }

    public interface IAttackMetaStrategy
    {
        public virtual int priority => 0;
        public virtual float actionMultiplier => 1;

        public (bool, bool) Attack(UnitModel unit, List<int> random, RoomAccessible neighbours,
            MultiValueDictionary<DVector2, UnitModel> positionsArr,
            IEnumerable<IAttackStrategy> attackStrategy, ExecutorWithPreview executor);
    }

    public interface IMovementMetaStrategy
    {
        public bool GetAttack(UnitModel unit, UnitModel otherUnit);
    }
}

public class SimpleAttack : UnitModel.IAttackMetaStrategy
{
    public virtual (bool, bool) Attack(UnitModel unit, List<int> random, RoomAccessible neighbours,
        MultiValueDictionary<DVector2, UnitModel> positionsArr,
        IEnumerable<UnitModel.IAttackStrategy> attackStrategy, ExecutorWithPreview executor)
    {
        foreach (var strategy in attackStrategy)
        {
            foreach (var move in neighbours.accessible.Keys)
            {
                if (!neighbours.StraightLine(move, unit.position)) continue;
                var otherUnits = positionsArr.GetValues(move, false);
                if (otherUnits == null) continue;
                foreach (var otherUnit in otherUnits.ToList())
                {
                    if (otherUnit.type == unit.type) continue;
                    if (!strategy.GetAttack(unit, otherUnit)) continue;
                    new Strike(unit, otherUnit).Attack(unit, otherUnit, executor);
                    unit.attack.Invoke(((move - unit.position).ToRotationFullOrLeft(), move, unit.unitData.attackVFX,
                        unit.unitData.impactVFX));
                    return (true, false);
                }
            }
        }

        return (false, false);
    }
}

public class Heal : UnitModel.IAttackMetaStrategy
{
    private int regenerationCounter;
    public int priority => -1;
    public float actionMultiplier => 2;

    public virtual (bool, bool) Attack(UnitModel unit, List<int> random, RoomAccessible neighbours,
        MultiValueDictionary<DVector2, UnitModel> positionsArr,
        IEnumerable<UnitModel.IAttackStrategy> attackStrategy, ExecutorWithPreview executor)
    {
        if (regenerationCounter > unit.unitData.power * 5) return (false, true);
        foreach (var strategy in attackStrategy)
        {
            foreach (var move in neighbours.accessible.Keys)
            {
                if (!neighbours.StraightLine(move, unit.position)) continue;
                var otherUnits = positionsArr.GetValues(move, false);
                if (otherUnits == null) continue;
                foreach (var otherUnit in otherUnits.ToList())
                {
                    if (otherUnit.type != unit.type) continue;
                    if (!strategy.GetAttack(unit, otherUnit)) continue;
                    if (otherUnit.hp.val == otherUnit.unitData.hp) continue;
                    regenerationCounter++;
                    var heal = new Strike(unit, otherUnit).initialDamage;
                    otherUnit.hp.val = Mathf.Min(otherUnit.hp.val + heal, otherUnit.unitData.hp);
                    unit.attack.Invoke(((move - unit.position).ToRotationFullOrLeft(), move, unit.unitData.attackVFX,
                        unit.unitData.impactVFX));
                    return (true, true);
                }
            }
        }

        return (false, false);
    }
}


public class Regen : UnitModel.IAttackMetaStrategy
{
    private int regenerationCounter;
    public int priority => -1;
    public float actionMultiplier => 1;

    public virtual (bool, bool) Attack(UnitModel unit, List<int> random, RoomAccessible neighbours,
        MultiValueDictionary<DVector2, UnitModel> positionsArr,
        IEnumerable<UnitModel.IAttackStrategy> attackStrategy, ExecutorWithPreview executor)
    {
        if (regenerationCounter > unit.unitData.power * 10) return (false, true);
        if (unit.hp.val == unit.unitData.hp) return (false, true);
        regenerationCounter++;
        unit.hp.val = Mathf.Min(unit.hp.val + 1, unit.unitData.hp);
        return (true, true);
    }
}

public class Rider : SimpleAttack
{
    public int priority => 1;
    public float actionMultiplier => 0.8f;

    public override (bool, bool) Attack(UnitModel unit, List<int> random, RoomAccessible neighbours,
        MultiValueDictionary<DVector2, UnitModel> positionsArr,
        IEnumerable<UnitModel.IAttackStrategy> attackStrategy, ExecutorWithPreview executor)
    {
        var (attacked, canMove) = base.Attack(unit, random, neighbours, positionsArr, attackStrategy, executor);
        return (attacked, true);
    }
}

public class Piercing : UnitModel.IAttackMetaStrategy
{
    public int priority => 1;
    public float actionMultiplier => 1.5f;

    public (bool, bool) Attack(UnitModel unit, List<int> random, RoomAccessible neighbours,
        MultiValueDictionary<DVector2, UnitModel> positionsArr,
        IEnumerable<UnitModel.IAttackStrategy> attackStrategy, ExecutorWithPreview executor)
    {
        UnitModel target = null;
        foreach (var strategy in attackStrategy)
        {
            foreach (var move in neighbours.accessible.Keys)
            {
                if (!neighbours.StraightLine(move, unit.position)) continue;
                var otherUnits = positionsArr.GetValues(move, false);
                if (otherUnits == null) continue;
                foreach (var otherUnit in otherUnits.ToList())
                {
                    if (otherUnit.type == unit.type) continue;
                    if (!strategy.GetAttack(unit, otherUnit)) continue;

                    new Strike(unit, otherUnit).Attack(unit, otherUnit, executor);
                    unit.attack.Invoke(((move - unit.position).ToRotationFullOrLeft(), move, unit.unitData.attackVFX,
                        unit.unitData.impactVFX));
                    target = otherUnit;
                    break;
                }
            }

            if (target != null)
                break;
        }

        if (target == null)
            return (false, false);

        foreach (DVector2 pos in neighbours.GetAllInLine(unit.position, target.position))
        {
            if (!positionsArr.TryGetValue(pos, out var otherUnits)) continue;
            foreach (var otherUnit in otherUnits.ToList())
            {
                if (otherUnit == target || otherUnit == unit) continue;
                new Strike(unit, otherUnit).Attack(unit, otherUnit, executor);
            }
        }

        return (true, false);
    }
}


public class Slasher : UnitModel.IAttackMetaStrategy
{
    public int priority => 1;

    public (bool, bool) Attack(UnitModel unit, List<int> random, RoomAccessible neighbours,
        MultiValueDictionary<DVector2, UnitModel> positionsArr,
        IEnumerable<UnitModel.IAttackStrategy> attackStrategy, ExecutorWithPreview executor)
    {
        UnitModel target = null;
        foreach (var strategy in attackStrategy)
        {
            foreach (var move in neighbours.accessible.Keys)
            {
                if (!neighbours.StraightLine(move, unit.position)) continue;
                var otherUnits = positionsArr.GetValues(move, false);
                if (otherUnits == null) continue;
                foreach (var otherUnit in otherUnits.ToList())
                {
                    if (otherUnit.type == unit.type) continue;
                    if (!strategy.GetAttack(unit, otherUnit)) continue;

                    new Strike(unit, otherUnit).Attack(unit, otherUnit, executor);
                    unit.attack.Invoke(((move - unit.position).ToRotationFullOrLeft(), move, unit.unitData.attackVFX,
                        unit.unitData.impactVFX));
                    target = otherUnit;
                    break;
                }
            }

            if (target != null)
                break;
        }

        if (target == null)
            return (false, false);

        foreach (var i in random)
        {
            if (!positionsArr.TryGetValue(unit.position + (i!=4? RotationUtils.rotations[i]:(0,0)), out var otherUnits)) continue;
            foreach (var otherUnit in otherUnits.ToList())
            {
                if (otherUnit == target || otherUnit == unit ||  otherUnit.type == unit.type ) continue;
                new Strike(unit, otherUnit).Attack(unit, otherUnit, executor);
            }
        }

        return (true, false);
    }
}


public class Melee : UnitModel.IAttackStrategy
{
    public bool GetAttack(UnitModel unit, UnitModel otherUnit)
    {
        return (unit.position - otherUnit.position).Magnitude <= 1;
    }
}

public class Ranged : UnitModel.IAttackStrategy
{
    public bool GetAttack(UnitModel unit, UnitModel otherUnit)
    {
        return (unit.position - otherUnit.position).Magnitude <= 2;
    }
}


public class GoToRoom : UnitModel.IMovementStrategy
{
    public bool skippable => false;

    public bool GetMove(UnitModel unit, RoomData neighbour, RoomData nextRoom,
        HashSet<UnitModel> inSight, DVector2 move, RoomData thisRoom, RoomData roomInSight)
    {
        return neighbour == nextRoom && unit.position != move;
    }
}

public class Aggressive : UnitModel.IMovementStrategy
{
    public bool GetMove(UnitModel unit, RoomData neighbour, RoomData nextRoom,
        HashSet<UnitModel> inSight, DVector2 move, RoomData thisRoom, RoomData roomInSight)
    {
        return inSight != null && inSight.Any(otherUnit => otherUnit.type != unit.type);
    }
}

public class Cautious : UnitModel.IMovementStrategy
{
    private int wait = 0;
    public bool skippable => false;

    public bool GetMove(UnitModel unit, RoomData neighbour, RoomData nextRoom,
        HashSet<UnitModel> inSight, DVector2 move, RoomData thisRoom, RoomData roomInSight)
    {
        return move == unit.position && Random.value > 0.5f;
    }
}

public class Ambush : UnitModel.IMovementStrategy
{
    public bool skippable => true;

    public bool GetMove(UnitModel unit, RoomData neighbour, RoomData nextRoom,
        HashSet<UnitModel> inSight, DVector2 move, RoomData thisRoom, RoomData roomInSight)
    {
        return neighbour == thisRoom && (roomInSight == thisRoom || roomInSight == null);
    }
}

public class FearEntrance : UnitModel.IMovementStrategy
{
    public bool GetMove(UnitModel unit, RoomData neighbour, RoomData nextRoom,
        HashSet<UnitModel> inSight, DVector2 move, RoomData thisRoom, RoomData roomInSight)
    {
        return neighbour.type != RoomType.Entrance;
    }
}


public class Fearful : UnitModel.IMovementStrategy
{
    public bool GetMove(UnitModel unit, RoomData neighbour, RoomData nextRoom,
        HashSet<UnitModel> inSight, DVector2 move, RoomData thisRoom, RoomData roomInSight)
    {
        return inSight == null || inSight.All(otherUnit => otherUnit.type == unit.type);
    }
}


public class WonderRandomlySameRoom : UnitModel.IMovementStrategy
{
    public bool skippable => false;

    public bool GetMove(UnitModel unit, RoomData neighbour, RoomData nextRoom,
        HashSet<UnitModel> inSight, DVector2 move, RoomData thisRoom, RoomData roomInSight)
    {
        return neighbour == thisRoom;
    }
}

public class WonderRandomlyNoBacktrackSameRoom : UnitModel.IMovementStrategy
{
    public bool skippable => false;

    public bool GetMove(UnitModel unit, RoomData neighbour, RoomData nextRoom,
        HashSet<UnitModel> inSight, DVector2 move, RoomData thisRoom, RoomData roomInSight)
    {
        return unit.lastPos != move && neighbour == thisRoom && unit.position != move;
    }
}
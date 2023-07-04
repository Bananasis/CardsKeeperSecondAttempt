using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

public class RoomAccessible
{
    public RoomData thisRoom { get; }
    private readonly GridData gd;


    public bool StraightLine(DVector2 first, DVector2 second)
    {
        if (first.x == second.x)
        {
            if (first.y == second.y) return true;
            var (max, min) = first.y > second.y ? (first.y, second.y) : (second.y, first.y);
            var pos = new DVector2(first.x, min);
            if (!GetRoom(pos, out var room)) return false;
            for (int y = min + 1; y <= max; y++)
            {
                var nextPos = new DVector2(first.x, y);
                if (!GetRoom(nextPos, out var nextRoom)) return false;
                if (nextRoom != room && !gd.IsOpenDoor(new GridDirection(nextPos, pos))) return false;
                pos = nextPos;
                room = nextRoom;
            }

            return true;
        }

        if (first.y == second.y)
        {
            var (max, min) = first.x > second.x ? (first.x, second.x) : (second.x, first.x);
            var pos = new DVector2(min, first.y);
            if (!GetRoom(pos, out var room)) return false;
            for (int x = min + 1; x <= max; x++)
            {
                var nextPos = new DVector2(x, first.y);
                if (!GetRoom(nextPos, out var nextRoom)) return false;
                if (nextRoom != room && !gd.IsOpenDoor(new GridDirection(nextPos, pos))) return false;
                pos = nextPos;
                room = nextRoom;
            }

            return true;
        }

        if (Mathf.Abs(first.x - second.x) == 1 && Mathf.Abs(first.y - second.y) == 1)
        {
            var corner1 = new DVector2(first.x, second.y);
            var corner2 = new DVector2(second.x, first.y);
            if (!GetRoom(first, out var firstRoom)) return false;
            if (!GetRoom(second, out var secondRoom)) return false;
            if (!GetRoom(corner1, out var firstCornerRoom)) return false;
            if (!GetRoom(corner2, out var secondCornerRoom)) return false;
            // if (firstRoom != firstCornerRoom && !gd.IsOpenDoor(new GridDirection(first, corner1))) return false;
            // if (firstRoom != secondCornerRoom && !gd.IsOpenDoor(new GridDirection(first, corner2))) return false;
            // if (secondRoom != firstCornerRoom && !gd.IsOpenDoor(new GridDirection(second, corner1))) return false;
            // if (secondRoom != secondCornerRoom && !gd.IsOpenDoor(new GridDirection(second, corner2))) return false;
            if (firstRoom != firstCornerRoom) return false;
            if (firstRoom != secondCornerRoom) return false;
            if (secondRoom != firstCornerRoom ) return false;
            if (secondRoom != secondCornerRoom ) return false;
            return true;
        }

        return false;
    }

    public RoomAccessible(GridData grid, RoomData room)
    {
        gd = grid;
        thisRoom = room;
        var roomPos = grid.GetPosition(room);
        foreach (var tile in room.tiles)
        {
            accessible.Add(tile.Key * roomPos, room);
        }

        foreach (var door in room.doors)
        {
            var dorPos = door * roomPos;
            if (!grid.IsOpenDoor(dorPos)) continue;
            if (!grid.GetRoom(dorPos.end, out var otherRoom)) throw new GameException("open Door but no room");
            accessible.TryAdd(dorPos.end, otherRoom);
            var nextRoomInLineDoor = dorPos.forward;
            if (!grid.GetRoom(nextRoomInLineDoor.end, out var nextRoomInLine)) continue;
            if (otherRoom != nextRoomInLine && !grid.IsOpenDoor(nextRoomInLineDoor)) continue;
            accessible.TryAdd(nextRoomInLineDoor.end, nextRoomInLine);
        }
    }

    public readonly Dictionary<DVector2, RoomData> accessible = new();


    public bool GetRoom(DVector2 move, out RoomData room)
    {
        return accessible.TryGetValue(move, out room);
    }

    public IEnumerable<DVector2> GetAllInLine(DVector2 unitPosition, DVector2 targetPosition)
    {
        var dir = targetPosition - unitPosition;
        dir = new DVector2(Math.Sign(dir.x), Math.Sign(dir.y));
        var line = new List<DVector2>();
        for (int i = 0; i < 3; i++)
        {
            var pos = unitPosition + dir * i;
            if (!StraightLine(unitPosition, pos)) break;
            line.Add(pos);
        }

        return line;
    }
}

public class GridData
{
    private DVector2 size => new (9, 9);
    private readonly Dictionary<RoomData, GridDirection> _rooms = new();
    private readonly Dictionary<DVector2, RoomData> occupiedTiles = new();
    private readonly Dictionary<RoomData, HashSet<RoomData>> roomConnections = new();
    private readonly Dictionary<GridDirection, RoomData> doors = new();
    public bool connected { get; private set; }

    public IReadOnlyDictionary<RoomData, GridDirection> rooms => _rooms;

    public GridDirection GetPosition(RoomData roomData)
    {
        return _rooms[roomData];
    }

    public Dictionary<RoomData, RoomAccessible> GetAllAccesibles()
    {
        return _rooms.Select((keyVal) => keyVal.Key).ToDictionary((r) => r, (r) => new RoomAccessible(this, r));
    }

    public Dictionary<RoomData, List<RoomData>> GetPaths(
        out Dictionary<(RoomData, RoomData), (HashSet<RoomData>, int)> allPaths)
    {
        Dictionary<RoomData, List<RoomData>> paths = new();
        var entrances = _rooms.Keys.Where(r => r.type == RoomType.Entrance).ToList();
        var allTreasuries = _rooms.Keys.Where(r => r.type == RoomType.Treasury).ToList();
        entrances.ForEach((e => paths.Add(e, new List<RoomData> {e})));
        var allClosestPaths = GetAllShortestPaths();
        foreach (var entrance in entrances)
        {
            RoomData startRoom = entrance;
            RoomData closest;
            RoomData roomToAdd;
            var treasuries = new List<RoomData>(allTreasuries);
            while (treasuries.Count != 0)
            {
                closest = treasuries.First(tr => allClosestPaths[(startRoom, tr)].Item2 ==
                                                 treasuries.Min(t => allClosestPaths[(startRoom, t)].Item2));
                treasuries.Remove(closest);
                roomToAdd = startRoom;
                while (roomToAdd != closest)
                {
                    roomToAdd = allClosestPaths[(roomToAdd, closest)].Item1.First();
                    paths[entrance].Add(roomToAdd);
                }

                startRoom = closest;
            }

            closest = entrances.First(tr => allClosestPaths[(startRoom, tr)].Item2 ==
                                            entrances.Min(t => allClosestPaths[(startRoom, t)].Item2));
            roomToAdd = startRoom;
            while (roomToAdd != closest)
            {
                roomToAdd = allClosestPaths[(roomToAdd, closest)].Item1.First();
                paths[entrance].Add(roomToAdd);
            }
        }

        allPaths = allClosestPaths;
        return paths;
    }

    private Dictionary<(RoomData, RoomData), (HashSet<RoomData>, int)> GetAllShortestPaths()
    {
        Dictionary<(RoomData, RoomData), (HashSet<RoomData>, int)> paths = new();
        foreach (var room in roomConnections.Keys)
        {
            paths[(room, room)] = (new HashSet<RoomData>() {room}, 0);
        }

        HashSet<RoomData> connectedRooms = new();
        foreach (var room in roomConnections.Keys)
        {
            connectedRooms.Clear();
            connectedRooms.Add(room);

            Queue<(RoomData, RoomData, int)> toConnect = new();
            foreach (var roomToConnect in roomConnections[room])
            {
                toConnect.Enqueue((roomToConnect, roomToConnect, 1));
            }

            while (toConnect.Count > 0)
            {
                var (dir, target, dist) = toConnect.Dequeue();
                if (connectedRooms.Contains(target)) continue;
                connectedRooms.Add(target);
                foreach (var roomData in roomConnections[target])
                {
                    if (connectedRooms.Contains(roomData)) continue;
                    toConnect.Enqueue((dir, roomData, dist + 1));
                }

                if (paths.TryGetValue((room, target), out var old))
                {
                    if (old.Item2 == dist)
                        old.Item1.Add(dir);
                    continue;
                }

                paths[(room, target)] = (new HashSet<RoomData> {dir}, dist);
            }
        }

        return paths;
    }

    private bool CheckConnectivity()
    {
        if (roomConnections.Count == 0) return true;
        var root = roomConnections.First().Key;
        HashSet<RoomData> connected = new();
        Queue<RoomData> toConnect = new Queue<RoomData>();
        toConnect.Enqueue(root);
        while (toConnect.Count > 0)
        {
            root = toConnect.Dequeue();
            if (connected.Contains(root)) continue;
            connected.Add(root);
            foreach (var roomData in roomConnections[root])
            {
                if (connected.Contains(roomData)) continue;
                toConnect.Enqueue(roomData);
            }
        }

        return _rooms.All((rd) => connected.Contains(rd.Key));
    }

    public bool GetRoom(DVector2 pos, out RoomData room)
    {
        return occupiedTiles.TryGetValue(pos, out room);
    }


    public bool IsOpenDoor(GridDirection door)
    {
        return doors.ContainsKey(door.Reverse()) && doors.ContainsKey(door);
    }


    public IEnumerable<RoomData> MoveRoom(GridDirection movementRoot, GridDirection gd)
    {
        if (!occupiedTiles.TryGetValue(movementRoot.start, out var room))
            throw new GameException("moved root is mot found");
        if (!rooms.TryGetValue(room, out var roomPos))
            throw new GameException("moved root is mot found");
        if (!CanBeMoved(movementRoot, gd, room, roomPos, out gd))
            throw new GameException("cant be moved");


        return RemoveRoom(room).Concat(AddRoom(room, gd));
    }

    public IEnumerable<RoomData> MoveRooms(GridDirection movementRoot, GridDirection gd,
        HashSet<RoomData> selectedRooms)
    {
        Dictionary<RoomData, GridDirection> roomPositions =
            new Dictionary<RoomData, GridDirection>(
                rooms.Where((keyVal) => selectedRooms.Contains(keyVal.Key)));
        if (!CanBeMoved(movementRoot, gd, roomPositions))
            throw new GameException("cant be moved");
        var affected = RemoveRooms(selectedRooms);
        foreach (var selectedRoom in selectedRooms)
        {
            affected.UnionWith(AddRoom(selectedRoom, roomPositions[selectedRoom], false));
        }

        connected = CheckConnectivity();
        return affected;
    }

    public HashSet<RoomData> AddRoom(RoomData roomData, GridDirection gd)
    {
        return AddRoom(roomData, gd, true);
    }

    private HashSet<RoomData> AddRoom(RoomData roomData, GridDirection gd, bool checkConnectivity)
    {
        if (!CanBeAdded(roomData, gd)) throw new GameException("Overlapping rooms");
        if (_rooms.ContainsKey(roomData)) throw new GameException("Room already on the grid");
        _rooms.Add(roomData, gd);
        roomConnections[roomData] = new HashSet<RoomData>();
        var newRoomDoors = roomData.doors.Select((door) => door * gd).ToList();
        doors.AddRange(newRoomDoors.Select(door => new KeyValuePair<GridDirection, RoomData>(door, roomData)));
        foreach (var door in newRoomDoors)
        {
            var reverseDoor = door.Reverse();
            if (!doors.TryGetValue(reverseDoor, out var otherRoom)) continue;
            roomConnections[roomData].Add(otherRoom);
            roomConnections[otherRoom].Add(roomData);
        }

        foreach (var tile in roomData.tiles.Keys)
        {
            occupiedTiles[(new GridDirection(tile) * gd).start] = roomData;
        }

        if (checkConnectivity)
            connected = CheckConnectivity();
        return new HashSet<RoomData>(roomConnections[roomData]);
    }

    public bool CanBeAdded(RoomData roomData, GridDirection gd)
    {
        foreach (var tilePos in roomData.tiles.Keys)
        {
            var gridPos = (new GridDirection(tilePos) * gd).start;
            if (gridPos.x > size.x || gridPos.y > size.y || gridPos.x < -size.x || gridPos.y < -size.y) return false;
            if (occupiedTiles.ContainsKey(gridPos))
                return false;
        }

        return true;
    }

    public bool CanBeMoved(GridDirection movementRoot, GridDirection gd, RoomData room, GridDirection roomPos,
        out GridDirection newRoomPosition)
    {
        newRoomPosition = (roomPos / movementRoot) * gd;
        foreach (var tilePos in room.tiles.Keys)
        {
            var gridPos = (new GridDirection(tilePos) * newRoomPosition).start;
            if (gridPos.x > size.x || gridPos.y > size.y || gridPos.x < -size.x || gridPos.y < -size.y) return false;
            if (occupiedTiles.TryGetValue(gridPos, out var occupied) &&
                occupied != room)
                return false;
        }

        return true;
    }

    public bool CanBeMoved(GridDirection movementRoot, GridDirection gd,
        Dictionary<RoomData, GridDirection> roomPositions)
    {
        bool canBeMoved = true;
        foreach (var selectedRoom in roomPositions.Keys.ToList())
        {
            var newRoomPosition = (roomPositions[selectedRoom] / movementRoot) * gd;
            roomPositions[selectedRoom] = newRoomPosition;
            foreach (var tilePos in selectedRoom.tiles.Keys)
            {
                var gridPos = (new GridDirection(tilePos) * newRoomPosition).start;
                if (gridPos.x > size.x || gridPos.y > size.y || gridPos.x < -size.x || gridPos.y < -size.y) 
                {
                    canBeMoved = false;
                    continue;
                }

                if (occupiedTiles.TryGetValue(gridPos, out var occupied) &&
                    !roomPositions.ContainsKey(occupied))
                    canBeMoved = false;
            }
        }

        return canBeMoved;
    }


    public HashSet<RoomData> RemoveRooms(HashSet<RoomData> selectedRooms)
    {
        HashSet<RoomData> affected = new HashSet<RoomData>();
        foreach (var selectedRoom in selectedRooms)
        {
            affected.UnionWith(RemoveRoom(selectedRoom));
        }

        connected = CheckConnectivity();
        affected.ExceptWith(selectedRooms);
        return affected;
    }
    
    public void RemoveAllRooms()
    {
        foreach (var room in _rooms.Keys.ToList())
        {
            RemoveRoom(room,false);
        }

        connected = CheckConnectivity();
    }

    public HashSet<RoomData> RemoveRoom(RoomData roomData)
    {
        return RemoveRoom(roomData, true);
    }


    private HashSet<RoomData> RemoveRoom(RoomData roomData, bool checkConnectivity)
    {
        if (!roomConnections.TryGetValue(roomData, out var connections))
            throw new GameException("Room already removed");
        roomConnections.Remove(roomData);
        foreach (var connection in connections)
        {
            if (!roomConnections[connection].Remove(roomData)) throw new GameException("Room already removed");
        }

        foreach (var door in roomData.doors)
        {
            var doorOnGrid = door * _rooms[roomData];
            if (!doors.Remove(doorOnGrid)) throw new GameException("Room already removed");
        }


        foreach (var tile in roomData.tiles.Keys)
        {
            var tileOnGrid = new GridDirection(tile) * _rooms[roomData];
            if (!occupiedTiles.Remove(tileOnGrid.start)) throw new GameException("Room already removed");
        }

        if (!_rooms.Remove(roomData)) throw new GameException("Room already removed");
        if (checkConnectivity)
            connected = CheckConnectivity();
        return connections;
    }
}


public enum RoomType
{
    Room,
    Treasury,
    Entrance,
}
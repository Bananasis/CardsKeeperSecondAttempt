using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

public partial class GridMono
{
    private bool TryRemoveSelectedFromGrid()
    {
        if (!_gridData.rooms.ContainsKey(selectedRooms.First())) return false;
        foreach (var roomData in _gridData.RemoveRooms(selectedRooms))
        {
            _rooms[roomData].Update();
        }

        isConnected.val = _gridData.connected;
        return true;
    }

    private void RemoveSelected()
    {
        TryRemoveSelectedFromGrid();

        foreach (var roomData in selectedRooms)
        {
            _gameManager.AddToHand(roomData, default);
            _rooms[roomData].destroy.val = true;
            _rooms[roomData].Dispose();
            _rooms.Remove(roomData);
            _gameManager.treasuries.val -= roomData.type == RoomType.Treasury ? 1 : 0;
        }

        selectedRooms.Clear();
    }

    private bool TryRemoveRoomFromGrid(Room room)
    {
        if (!_gridData.rooms.ContainsKey(room.data)) return false;
        foreach (var roomData in _gridData.RemoveRoom(room.data))
        {
            _rooms[roomData].Update();
        }

        return true;
    }

    public void RemoveRoom(Room room, bool returnToHand)
    {
        if (!selectedRooms.Contains(room.data))
            DeselectAll();
        if (selectedRooms.Count > 0)
        {
            RemoveSelected();
            return;
        }

        TryRemoveRoomFromGrid(room);
        if (returnToHand)
            _gameManager.AddToHand(room.data, default);
        if (!_rooms.Remove(room.data)) throw new GameException("room is not on gridMono");
        _gameManager.treasuries.val -= room.data.type == RoomType.Treasury ? 1 : 0;
        room.destroy.val = true;
        room.Dispose();
    }

    public void RemoveAllRooms(bool returnToHand = false)
    {
        DeselectAll();
        foreach (var room in _rooms.Values.ToList())
        {
            RemoveRoom(room, returnToHand);
        }
    }

    public void TryRemoveRoom(Room room,bool returnToHand)
    {
        if (_room == room) return;
        RemoveRoom(room,returnToHand);
    }

    private bool TryAddRoomToGrid(Room room, GridDirection gridDirection)
    {
        if (!_gridData.CanBeAdded(room.data, gridDirection)) return false;
        foreach (var roomData in _gridData.AddRoom(room.data, gridDirection))
        {
            _rooms[roomData].Update();
        }

        isConnected.val = _gridData.connected;
        return true;
    }

    private void AddRoom(RoomData selectedRoom, GridDirection gridDirection, out Room newRoom)
    {
        newRoom = new Room(selectedRoom, _gridData);
        newRoom.onGrid.val = TryAddRoomToGrid(newRoom, gridDirection);
        newRoom.Update(gridDirection);
        _rooms.Add(selectedRoom, newRoom);
        _gameManager.treasuries.val += selectedRoom.type == RoomType.Treasury ? 1 : 0;
    }
    
    public void AddRoom(RoomData roomData, GridDirection gridDirection)
    {
        AddRoom(roomData, gridDirection, out var room);
        _roomPool.Get(transform).UpdateRoom(room);
       // MoveRoom(_room, _room.position.val, gridDirection);
    }

    private bool TryMoveSelectedOnGrid(GridDirection root, GridDirection gridDirection,
        out Dictionary<RoomData, GridDirection> newPositions)
    {
        newPositions = selectedRooms.ToDictionary((room) =>
            room, (room) => _rooms[room].position.val);
        var anyRoomPos = newPositions.First();
        if (!_gridData.CanBeMoved(root, gridDirection, newPositions)) return false;

        foreach (var roomData in _gridData.MoveRooms(root / anyRoomPos.Value * _gridData.rooms[anyRoomPos.Key],
                     gridDirection, selectedRooms))
        {
            _rooms[roomData].Update();
        }

        isConnected.val = _gridData.connected;
        return true;
    }

    private bool TryMoveRoomOnGrid(Room room, GridDirection root, GridDirection gridDirection,
        out GridDirection newPosition)
    {
        if (!_gridData.rooms.ContainsKey(room.data))
        {
            newPosition = gridDirection;
            return TryAddRoomToGrid(room, gridDirection);
        }

        if (!_gridData.CanBeMoved(root, gridDirection, room.data, room.position.val, out newPosition))
            return false;

        foreach (var roomData in _gridData.MoveRoom(root / room.position.val * _gridData.rooms[room.data],
                     gridDirection))
        {
            _rooms[roomData].Update();
        }

        isConnected.val = _gridData.connected;
        return true;
    }

    private void MoveSelected(GridDirection root, GridDirection gridDirection)
    {
        var moved = TryMoveSelectedOnGrid(root, gridDirection, out var newPositions);
        foreach (var roomToUpdate in selectedRooms)
        {
            _rooms[roomToUpdate].Update(newPositions[roomToUpdate]);
            _rooms[roomToUpdate].onGrid.val = moved;
        }
    }

    public void MoveRoom(Room room, GridDirection root, GridDirection gridDirection)
    {
        if (!selectedRooms.Contains(room.data))
            DeselectAll();
        if (root == gridDirection) return;
        if (selectedRooms.Count > 0)
        {
            MoveSelected(root, gridDirection);
            return;
        }

        room.onGrid.val = TryMoveRoomOnGrid(room, root, gridDirection, out var newPosition);
        room.Update(newPosition);
    }
}
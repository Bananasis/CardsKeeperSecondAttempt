using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public partial class GridMono
{
    public void ResetSelected()
    {
        foreach (var selectedRoom in selectedRooms)
        {
            _rooms[selectedRoom].Update(_gridData.GetPosition(selectedRoom));
            _rooms[selectedRoom].onGrid.val = true;
        }
    }

    public void DeselectAll()
    {
        foreach (var roomData in selectedRooms.ToList())
        {
            Select(roomData, false);
        }
    }


    public void Select(RoomData selectedRoom, bool select)
    {
        if (select)
        {
            if (!selectedRooms.Add(selectedRoom)) throw new GameException("already selected!");
        }
        else
        {
            if (!selectedRooms.Remove(selectedRoom)) throw new GameException("not selected!");
        }

        _rooms[selectedRoom].selected.val = select;
    }

    public void ResetPositions(Room room)
    {
        if (selectedRooms.Contains(room.data))
        {
            ResetSelected();
            return;
        }
        _rooms[room.data].onGrid.val = true;
        room.Update(_gridData.GetPosition(room.data));
    }

    public void SelectAll(Rect rect)
    {
        for (int i = Mathf.CeilToInt(rect.min.x); i <= rect.max.x; i++)
        {
            for (int j = Mathf.CeilToInt(rect.min.y); j <= rect.max.y; j++)
            {
               if(! _gridData.GetRoom(new DVector2(i, j),out var room)) continue;
               if (!selectedRooms.Contains(room))
               {
                   Select(room,true);
               }
            }
        }
    }
}
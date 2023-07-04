using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

public class Room : IDisposable
{
    private RoomData _data;

    public RoomData data
    {
        get => _data;
    }

    public readonly List<Tile> tiles = new List<Tile>();
    public readonly RootCell<GridDirection> position = new RootCell<GridDirection>();
    public readonly RootCell<bool> selected = new RootCell<bool>();
    public readonly RootCell<bool> onGrid = new RootCell<bool>();
    public readonly RootCell<bool> destroy = new RootCell<bool>();

    public Room(RoomData data, GridData gd)
    {
        _data = data;
        tiles.AddRange(data.tiles.Values.Select((tileData) => new Tile(gd, tileData)));
    }

    public void Update(GridDirection pos)
    {
        position.val = pos;
        tiles.ForEach(t => t.Update(pos));
    }

    public void Update()
    {
        tiles.ForEach(t => t.Update(position.val));
    }

    public void Dispose()
    {
        tiles.ForEach((tile) => tile.Dispose());
        tiles.Clear();
        selected.Dispose();
        onGrid.Dispose();
        position.Dispose();
        destroy.Dispose();
    }
}
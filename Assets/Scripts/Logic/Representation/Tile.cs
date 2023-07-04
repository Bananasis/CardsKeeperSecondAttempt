using System;
using System.Linq;
using Utils;

public class Tile : IDisposable
{
    public Cell<GridDirection> position = new RootCell<GridDirection>();
    public CellDict<GridDirection, bool> doorsOpened = new();

    private TileData _data;

    public TileData data => _data;
    private GridData _gd;

    public Tile(GridData gd, TileData data)
    {
        _gd = gd;
        _data = data;
        foreach (var gridDirection in _data.doors)
        {
            doorsOpened[gridDirection] = false;
        }
    }


    public void Update(GridDirection position)
    {
        this.position.val = new GridDirection(data.position, Rotation.Left) * position;
        foreach (var gridDirection in _data.doors)
        {
            doorsOpened[gridDirection] = _gd.IsOpenDoor(gridDirection * position);
        }
    }


    public void Dispose()
    {
        position?.Dispose();
        doorsOpened?.Dispose();
    }

    public int GetWallCount()
    {
        if (!_gd.GetRoom(position.val.start, out var thisRoom)) throw new Exception("");
        return RotationUtils.rotations.Count((rot) =>
        {
            _gd.GetRoom(position.val.start + rot, out var otherRoom);
            return otherRoom != thisRoom;
        });
    }
}
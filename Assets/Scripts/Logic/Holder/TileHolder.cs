using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class TileHolder : MonoBehaviour
{
    [SerializeField] private List<TileDecorationSprite> tileDecorations = new();
    [SerializeField] private List<TileWallSprite> tileWalls = new();
    
    public readonly Dictionary<RoomDecorationType, TileComponentSprite> tileDecorationsDict = new();
    public readonly Dictionary<WallType, TileComponentSprite> tileWallsDict = new();

    public void Awake()
    {
        tileDecorations.ForEach((tile) => tileDecorationsDict[tile.type] = tile);
        tileWalls.ForEach((tile) => tileWallsDict[tile.type] = tile);
    }


  

    public static RoomDecorationType RandomDecoration(float f)
    {
        if (Random.value > f) return RoomDecorationType.Floor;
        return (RoomDecorationType) Random.Range((int) RoomDecorationType.Crack1, (int) RoomDecorationType.Entrance);
    }
}

[Serializable]
public class TileComponentSprite
{
    public int layer;
    public RotationType rType;
    public Sprite sprite;
    public Sprite[] rotations = new Sprite[4];
    public Rotation rotation;
}

[Serializable]
class TileWallSprite : TileComponentSprite
{
    public WallType type;
}
[Serializable]
class TileDecorationSprite : TileComponentSprite
{
    public RoomDecorationType type;
}

public enum RotationType
{
    None,
    Multiple,
    Single
}


public enum RoomDecorationType
{
    Floor,
    MoneyBig,
    MoneyMedium,
    MoneySmall,
    DoorOpen,
    DoorClosed,
    DoorBricked,
    Crack1,
    Crack2,
    Crack3,
    Stone1,
    Stone2,
    Stone3,
    Entrance,
}

public enum WallType
{
    Empty,
    Corner,
    IType,
    UType,
    HType,
    LType,
    Full
}
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Utils;

public class TileData
{
    // public class TileDataSave
    // {
    //     public List<GridDirection> doors;
    //     public DVector2 position;
    //     public RoomDecorationType decoration;
    //     public string mobID;
    //
    //     public TileData Load(RoomHolder rh,IEnumerable<FullRotation> sameRoom)
    //     {
    //         var td = new TileData(position, doors, decoration, sameRoom);
    //         if (mobID != "") td.AddMob(rh.GetMobData(mobID));
    //         return td;
    //     }
    // }
    //
    // public TileDataSave Save()
    // {
    //     return new TileDataSave()
    //     {
    //         doors = doors.ToList(),
    //         position =   position,
    //         decoration = decoration,
    //         mobID = mob == null?"":mob.mobName
    //     };
    // }

    public DVector2 position;
    public List<GridDirection> doors;
    public bool[] sameRoom = new bool[8];
    public RoomDecorationType decoration;
    
    [JsonProperty]
    public MobData mob { get; private set; }

    public void AddMob(MobData newMob)
    {
        if (mob != null) throw new GameException("mob already in the room");
        mob = newMob;
    }

    public MobData RemoveMob()
    {
        var oldMob = mob;
        mob = null;
        return oldMob;
    }
    [JsonConstructor]
    public TileData()
    {
    }

    public TileData(DVector2 position, IEnumerable<GridDirection> doors, RoomDecorationType decoration,
        IEnumerable<FullRotation> sameRoom)
    {
        this.position = position;
        this.decoration = decoration;
        this.doors = new List<GridDirection>(doors);
        for (var i = 0; i < this.sameRoom.Length; i++)
        {
            this.sameRoom[i] = false;
        }

        foreach (var fullRotation in sameRoom)
        {
            this.sameRoom[(int) fullRotation] = true;
        }
    }
}
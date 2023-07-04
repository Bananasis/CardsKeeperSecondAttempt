using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "Mob", menuName = "ScriptableObjects/Mob", order = 1)]
public class MobData : ScriptableObject
{
    public Sprite sprite;
    public string mobName;
    public List<Tag> tags;
    public List<MergeData> crafts;
    public int cost;
    public int tier;
}

public class MobConverter : JsonConverter<MobData>
{
    private RoomHolder _roomHolder;
    

    public MobConverter(RoomHolder rh)
    {
        _roomHolder = rh;
    }

    public override void WriteJson(JsonWriter writer, MobData value, JsonSerializer serializer)
    {
        writer.WriteValue(value == null ? "" : value.name);
    }

    public override MobData ReadJson(JsonReader reader, Type objectType, MobData existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.Value == null) return null;
        string s = (string) reader.Value;
        if (s == "") return null;
        return _roomHolder.GetMobData(s);
    }
}
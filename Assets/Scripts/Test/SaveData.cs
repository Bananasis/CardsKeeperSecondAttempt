using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;


public class SaveData
{
    [Inject] private GameManager _gameManager;
    [Inject] private GridData _grid;
    [Inject] private GridMono _gridMono;
    [Inject] private Hand _hand;
    [Inject] private RoomHolder rh;

    public int level;
    public int money;
    public List<RoomData> hand;
    public List<(GridDirection, RoomData)> field;

    public void Delete()
    {
        PlayerPrefs.DeleteKey("Save");
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey("Save");
    }

    public void Save()
    {
        level = _gameManager.level.val;
        hand = _hand.allRooms.ToList();
        field = _grid.rooms.Select(pair => (pair.Value, pair.Key)).ToList();
        money = _gameManager.money.val;
        var json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            Converters = new List<JsonConverter> {new MobConverter(rh)}
        });
        PlayerPrefs.SetString("Save", json);
    }

    public bool Load()
    {
        var json = PlayerPrefs.GetString("Save", "");
        if (json == "")
        {
            return false;
        }

        var save = JsonConvert.DeserializeObject<SaveData>(json, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            Converters = new List<JsonConverter> {new MobConverter(rh)}
        });
        if (save == null) throw new Exception("Save is corrupted");

        _gameManager.money.val = save.money;
        _gameManager.level.val = save.level;
        _gridMono.RemoveAllRooms();
        save.field.ForEach((tuple) => _gridMono.AddRoom(tuple.Item2, tuple.Item1));
        _hand.Clear();
        save.hand.ForEach(c => _hand.AddCard(c, Vector3.zero, true));
        return true;
    }
};
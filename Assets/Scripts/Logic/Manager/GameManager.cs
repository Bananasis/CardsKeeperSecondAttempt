using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private Hand handDisplay;
    [Inject] private WaveDisplay waveDisplay;
    [Inject] private WaveHolder waveHolder;
    [Inject] private RoomHolder roomHolder;
    [Inject] private GridMono _gridMono;
    [Inject] private FightManager _fightManager;
    [Inject] private SaveData save;
    
    private List<Enemy> enemies = new();
    private int _treasuries;


    public readonly Cell<int> treasuries = new RootCell<int>();
    public readonly Cell<int> money = new RootCell<int>();
    public readonly Cell<bool> canStartNewWave = new RootCell<bool>();
    public UnityEvent OnVictory = new();
    public UnityEvent OnDefeat = new();
    public RootCell<int> level = new RootCell<int>();

    public void Start()
    {
        
        money.Subscribe((m) =>
        {
            if (m <= 0) Loose();
        }, preInvoke: false);
        _gridMono.isConnected.Subscribe((_) =>
            canStartNewWave.val = _gridMono.isConnected.val && handDisplay.onlyRoomCards.val);
        handDisplay.onlyRoomCards.Subscribe((_) =>
            canStartNewWave.val = _gridMono.isConnected.val && handDisplay.onlyRoomCards.val);
        if (save.HasSave())
        {
            save.Load();
            NextWave();
            return;
        }
        money.val = 3;
        AddToHand(roomHolder.GetRoom(RoomType.Entrance), default);
        waveHolder.startHand.ForEach(mData => AddToHand(roomHolder.GetRoom(RoomType.Room, mData), default));
        AddToHand(roomHolder.GetRoom(RoomType.Treasury), default);
        NextWave();
    }

    private void Loose()
    {
        save.Delete();
        OnDefeat.Invoke();
    }


    private WaveData wave = null;

    public void StartWave()
    {
        if (!canStartNewWave.val) return;
        save.Save();
        money.val = Math.Min(money.val, treasuries.val * 5);
        _fightManager.StartWave(wave.wave.Select((tuple) => roomHolder.GetEnemy(tuple.weapon, tuple.enemy))
            .ToList());
    }

    public void NextWave(int treasuriesNonEmpty = 0)
    {
        
    
        if (wave != null)
        {
            level.val++;
            money.val += treasuriesNonEmpty +
                         (treasuriesNonEmpty == treasuries.val ? money.val / 10 + (money.val >= 5 ? 1 : 0) : 0);
            wave.reward.ForEach((mt) => AddToHand(roomHolder.GetRoom(RoomType.Room, mt), default));
            if (wave is {treasureReward: true})
            {
                AddToHand(roomHolder.GetRoom(RoomType.Treasury), default);
            }

            if (wave is {entranceReward: true})
            {
                AddToHand(roomHolder.GetRoom(RoomType.Entrance), default);
            }
        }

        wave = waveHolder.GetWave(level.val);
        if (wave == null)
        {
            Win();
            return;
        }

        enemies = wave?.wave.Select((e) => roomHolder.GetEnemy(e.weapon, e.enemy)).ToList();
        waveDisplay.UpdateWave(enemies);
      
    }

    private void Win()
    {
       OnVictory.Invoke();
    }


    public void Sell(RoomCardUI card)
    {
        if (card.room.type != RoomType.Room) throw new GameException("cannot sell entrances and treasuries");
        card.Dispose();
        money.val += card.room.mobs.Sum(mob =>
            mob.cost < 3 ? mob.cost : Mathf.CeilToInt(mob.cost * 2f / 3f));
    }


    public void AddToHand(RoomData room, Vector3 startPosition, bool anchored = true)
    {
        handDisplay.AddCard(room, startPosition, anchored);
    }

    public void QuitToMenu()
    {
        if(!_fightManager.battle.val)
            save.Save();
        SceneManager.LoadScene(0);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

public class StoreMenu : MonoBehaviour
{
    [SerializeField] private List<StoreSlot> slots;
    private readonly Dictionary<StoreSlot, IDisposable> slotChecks = new();
    [SerializeField] private Button cancel;
    [SerializeField] private RectTransform rect;
    private readonly Dictionary<StoreSlot, RoomCardUI> _roomsCards = new();
    [Inject] private GameManager _gameManager;
    [Inject] private RoomHolder _roomHolder;
    [Inject] private WaveHolder _waveHolder;
    [Inject] private Hand _hand;
    public bool buying;


    private void Start()
    {
        _gameManager.money.Subscribe(SetInteractable);
        cancel.onClick.Subscribe(Cancel);
    }

    private void SetInteractable(int money)
    {
        foreach (var keyValuePair in _roomsCards)
        {
            keyValuePair.Value.LockInteractable(keyValuePair.Value.room.mobs.First().cost < money);
        }
    }

    public void StartBuy()
    {
        if (buying) return;
        if (_gameManager.money.val < 2) return;
        gameObject.SetActive(true);
        buying = true;
        UpdateStore(_waveHolder.mobPool);
    }

    IEnumerator WaitAndDo(Action act)
    {
        yield return null;
        act.Invoke();
    }

    private void Cancel()
    {
        Clear();
        buying = false;
        gameObject.SetActive(false);
    }

    private void Clear()
    {
        foreach (var keyValuePair in _roomsCards)
        {
            keyValuePair.Value.Dust();
        }

        foreach (var keyValuePair in slotChecks)
        {
            keyValuePair.Value.Dispose();
        }

        slotChecks.Clear();

        _roomsCards.Clear();
    }

    private void AddToSlot(StoreSlot slot, MobData mob)
    {
        var card = _hand.AddCard(_roomHolder.GetRoom(RoomType.Room, mob), slot.position, false, false);
        if (slotChecks.ContainsKey(slot)) throw new GameException("");
        slotChecks.Add(slot, card.draggedCell.Subscribe((d) =>
        {
            if (!d) return;
            TryBuy(slot, card);
        },preInvoke:false));
        card.position.preferredWorldPosition = slot.position;
        card.UpdateSortingOrder(29);
        card.LockInteractable(mob.cost < _gameManager.money.val);
        _roomsCards.Add(slot, card);
    }

    private void UpdateStore(List<MobData> mobsData)
    {
        slots.ForEach(s => s.gameObject.SetActive(false));
        for (var index = 0; index < mobsData.Count && index < slots.Count; index++)
        {
            slots[index].gameObject.SetActive(true);
        }

        StartCoroutine(WaitAndDo(() =>
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            for (var index = 0; index < mobsData.Count && index < slots.Count; index++)
            {
                AddToSlot(slots[index], mobsData[index]);
                slots[index].SetPrice(mobsData[index].cost);
            }
        }));
    }

    public bool CardInSlot(StoreSlot storeSlot, RoomCardUI drop)
    {
        return _roomsCards[storeSlot] == drop;
    }

    public void TryBuy(StoreSlot storeSlot, RoomCardUI drop)
    {
        if (_roomsCards[storeSlot] != drop) return;
        var mob = drop.room.mobs.First();
        if (mob.cost >= _gameManager.money.val) throw new GameException("");
        _gameManager.money.val -= mob.cost;
        if (!_roomsCards.Remove(storeSlot)) throw new GameException("");
        slotChecks[storeSlot].Dispose();
        slotChecks.Remove(storeSlot);
        AddToSlot(storeSlot, mob);
    }
}
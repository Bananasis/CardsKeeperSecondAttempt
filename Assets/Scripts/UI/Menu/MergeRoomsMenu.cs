using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

public class MergeRoomsMenu : MonoBehaviour
{
    [SerializeField] private CardMergeSlot firstSlot;
    [SerializeField] private CardMergeSlot secondSlot;
    [SerializeField] private CardMergeSlot resultRect;
    [SerializeField] private Button confirm;
    [SerializeField] private Button cancel;
    [Inject] private Hand _hand;
    [Inject] private RoomHolder _roomHolder;
    [Inject] private GameManager _gameManager;
    private IDisposable firstCheck;
    private IDisposable secondCheck;
    private IDisposable resultCheck;
    private RoomCardUI first;
    private RoomCardUI second;
    private RoomCardUI result;
    public bool merging { get; private set; }

    public void Awake()
    {
        confirm.onClick.Subscribe(() => Merge(true));
        _gameManager.money.Subscribe(m => MakeResultInteractable(m > 1));
        cancel.onClick.Subscribe(Cancel);
    }

    private bool resultInteractable;
    private void MakeResultInteractable(bool b)
    {
        resultInteractable = b;
        result?.LockInteractable(b);
    }

    public void CheckForMenuCollision(RoomCardUI cardUI)
    {
        if (cardUI == first || cardUI == second)
        {
            Cancel();
        }

        if (cardUI == result)
        {
            Merge(true);
        }
    }

    public void BeginMerge(RoomCardUI cardUI)
    {
        gameObject.SetActive(true);
        merging = true;
        if (first == null)
        {
            SetFirst(cardUI);
            return;
        }

        if (second == null)
        {
            SetSecond(cardUI);
            return;
        }

        SetFirst(cardUI);
    }

    private void SetFirst(RoomCardUI cardUI)
    {
        cardUI.Hold();
        cardUI.position.preferredWorldPosition = firstSlot.position;
        if (cardUI == first) return;
        if (first != null)
        {
            first.Release();
            firstCheck.Dispose();
        }

        first = cardUI;
        firstCheck = first.draggedCell.Subscribe((d) =>
        {
            if (!d) return;
            ClearSlot(firstSlot, first);
        }, preInvoke: false);
        UpdateMerge();
    }

    private void SetSecond(RoomCardUI cardUI)
    {
        cardUI.Hold();
        cardUI.position.preferredWorldPosition = secondSlot.position;
        if (cardUI == second) return;
        if (second != null)
        {
            second.Release();
            secondCheck.Dispose();
        }

     
        second = cardUI;
        secondCheck = second.draggedCell.Subscribe((d) =>
        {
            if (!d) return;
            ClearSlot(secondSlot, second);
        }, preInvoke: false);
        UpdateMerge();
    }

    private void UpdateMerge()
    {
        if (result != null)
        {
            result.Dust();
            resultCheck.Dispose();
        }

        result = null;
        if (first == null || second == null || !_roomHolder.CanBeMerged(first.room, second.room))
        {
            return;
        }

        result = _hand.AddCard(_roomHolder.Merge(first.room, second.room), resultRect.position, false, false);
        result.LockInteractable(resultInteractable);
        resultCheck = result.draggedCell.Subscribe((d) =>
        {
            if (!d) return;
            ClearSlot(resultRect, result);
        }, preInvoke: false);
        result.position.preferredWorldPosition = resultRect.position;
        result.UpdateSortingOrder(30);
    }


    private void Merge(bool releaseResult)
    {
        if (_gameManager.money.val <= 1) throw new GameException("");
        if (result == null) return;
        
        if (releaseResult)
            result.Release();
        firstCheck.Dispose();
        secondCheck.Dispose();
        resultCheck.Dispose();
        firstCheck = null;
        secondCheck = null;
        resultCheck = null;
        first.Dust();
        second.Dust();
        second = null;
        first = null;
        result = null;
        merging = false;
        gameObject.SetActive(false);
        _gameManager.money.val -= 1;
    }

    private void Cancel()
    {
        
        firstCheck?.Dispose();
        secondCheck?.Dispose();
        resultCheck?.Dispose();
        firstCheck = null;
        secondCheck = null;
        resultCheck = null;
        first?.Release();
        second?.Release();
        result?.Dust();
        second = null;
        first = null;
        result = null;
        merging = false;
        gameObject.SetActive(false);
    }

    public bool CanBeMergedBySlot(CardMergeSlot cardMergeSlot, RoomCardUI card)
    {
        if (card.room.type != RoomType.Room) return false;
        if (cardMergeSlot == firstSlot)
        {
            if (second == null) return _roomHolder.CanBeMerged(card.room);
            return _roomHolder.CanBeMerged(card.room, second.room);
        }

        if (cardMergeSlot == secondSlot)
        {
            if (first == null) return _roomHolder.CanBeMerged(card.room);
            return _roomHolder.CanBeMerged(first.room, card.room);
        }

        return card == result && cardMergeSlot == resultRect;
    }

    public void AddBySlot(CardMergeSlot cardMergeSlot, RoomCardUI drop)
    {
        if (cardMergeSlot == firstSlot)
        {
            SetFirst(drop);
        }

        if (cardMergeSlot == secondSlot)
        {
            SetSecond(drop);
        }

        if (cardMergeSlot != resultRect || drop != result) return;
        drop.Hold();
        drop.position.preferredWorldPosition = resultRect.position;
    }

    public void ClearSlot(CardMergeSlot cardMergeSlot, RoomCardUI roomCardUI)
    {
        if (cardMergeSlot == firstSlot && roomCardUI == first)
        {
            first = null;
            UpdateMerge();
            return;
        }

        if (cardMergeSlot == secondSlot && roomCardUI == second)
        {
            second = null;
            UpdateMerge();
            return;
        }

        if (cardMergeSlot != resultRect || roomCardUI != result) return;
        Merge(false);
    }

    public bool CanBeMerged(RoomCardUI card)
    {
        return CanBeMergedBySlot(firstSlot, card) || CanBeMergedBySlot(secondSlot, card);
    }
}
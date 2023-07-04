using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

public class Hand : DynamicHand<RoomCardUI>
{
    [SerializeField] private RoomCard cardPref;
    [Inject] RoomCardUI.MonoPool _pool;
    public readonly Cell<bool> onlyRoomCards = new RootCell<bool>();
    private int nonRoomCards;
    public IEnumerable<RoomData> allRooms => cards.Select(c => c.room);



    protected override void OnAddToHand(RoomCardUI card)
    {
        nonRoomCards += card.room.type != RoomType.Room ? 1 : 0;
        onlyRoomCards.val = nonRoomCards == 0;
    }

    protected override void OnRemoveFromHand(RoomCardUI card)
    {
        nonRoomCards -= card.room.type != RoomType.Room ? 1 : 0;
        onlyRoomCards.val = nonRoomCards == 0;
    }

    public RoomCardUI AddCard(RoomData room, Vector3 pos, bool local, bool toHand = true)
    {
        var card = _pool.Get(transform);
        card.Init();
        card.UpdateCard(room);
        if (!toHand) card.Hold();
        card.position.selfManaged = true;
        card.position.lockedRotation = false;
        if (local)
            card.position.currentPosition = pos;
        else
            card.position.currentWorldPosition = pos;
        card.position.preferredRotation = Quaternion.identity;
        TryAddToHand(card);
        return card;
    }

    public void Clear()
    {
        cards.ToList().ForEach(c => c.Dispose());
    }
    
    
}
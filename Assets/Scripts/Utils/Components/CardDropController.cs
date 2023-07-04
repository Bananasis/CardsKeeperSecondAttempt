using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace DefaultNamespace
{
    public class CardDropController : DroppableOn<CardDropZone>
    {
        [SerializeField] private RoomCardUI _roomCardUI;
        [Inject] private IVanishController vanish;


        protected override void OnBeginDrag(PointerEventData eventData, CardDropZone drop)
        {
            if(_roomCardUI.disposing) return;
            vanish.Vanish(drop.dropVanish);
            
        }

        protected override void OnEndDrag(PointerEventData eventData, CardDropZone drop, bool dropped)
        {
          
            if(_roomCardUI.disposing) return;
            vanish.Vanish(0);
            if(!dropped) return;
            _roomCardUI.Drop(!drop.TakeControl(_roomCardUI));
        }

        protected override void OnDrag(PointerEventData eventData, bool withDropZone)
        {
            _roomCardUI.Move(eventData.delta);
            
        }

        protected override void OnBeginDrag(PointerEventData eventData, bool withDropZone)
        {
            _roomCardUI.Drag();
        }

        protected override void OnEndDrag(PointerEventData eventData, bool withDropZone)
        {
            if(withDropZone) return;
            _roomCardUI.Drop(true);
        }
    }
}
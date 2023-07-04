using System;
using DefaultNamespace;
using UI.Menu;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


    public class RoomCardUI : CardUI<Hand, RoomCardUI>
    {
        [SerializeField] private RoomCard roomCard;
        [Inject] private IVanishController _vanish;
        [SerializeField] private Button mergeMobs;
        [Inject] private MergeMobsMenu _mergeMobs;
        [Inject] private MergeRoomsMenu _mergeRoomsMenu;
        [Inject] private RoomHolder _roomHolder;

        public bool disposing
        {
            get;
            protected set;
        }

        public RoomData room => roomCard.room;

        private void Awake()
        {
            mergeMobs.onClick.Subscribe(() =>
            {
                _mergeRoomsMenu.CheckForMenuCollision(this);
                _mergeMobs.BeginMerge(this);
            });
        }

        public override void Dispose()
        {
            _vanish.VanishInstant(0);
            disposing = false;
            base.Dispose();
        }

        public void UpdateCard(RoomData room)
        {
            mergeMobs.gameObject.SetActive(_roomHolder.HaveMobsToUpgrade(room));
            roomCard.UpdateCard(room);
        }


        public void Move(Vector2 eventDataDelta)
        {
            position.currentPosition += (Vector3) eventDataDelta / _canvas.scaleFactor;
            position.OnNeedPositionUpdate.Invoke();
        }
        
        public void Drag()
        {
            LockInteractable(false);
            inHand = false;
            dragged = true;
            position.selfManaged = false;
            Hover();
            position.bezierSize = 0.5f;
            
        }

        public void Drop(bool returnToHand)
        {
            LockInteractable(true);
            inHand = returnToHand;
            dragged = false;
            position.selfManaged = true;
            UnHover();
            position.bezierSize =returnToHand? position.bezierSize:0;
            position.OnNeedPositionUpdate.Invoke();
            
          
        }

        public void Release()
        {
            inHand = true;
            position.OnNeedPositionUpdate.Invoke();
        }
        
        public void Hold()
        {
            inHand = false;
            position.OnNeedPositionUpdate.Invoke();
        }

        public void Dust()
        {
            disposing = true;
            Hold();
            LockInteractable(false);
            _vanish.Vanish(1,Dispose);
        }
    }

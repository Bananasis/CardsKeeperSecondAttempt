using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class HandDropZone : CardDropZone
    {
        [SerializeField] private Hand hand;
        public override float dropVanish => 0;

        protected override void OnPointerEnter(PointerEventData data, RoomCardUI drop)
        {
            drop.position.bezierSize = 1;
            hand.TryAddToHand(drop);
        }

        protected override void OnPointerMove(PointerEventData data, RoomCardUI drop)
        {
            hand.TryAddToHand(drop);
        }

        protected override void OnPointerExit(PointerEventData data, RoomCardUI drop, bool dropped)
        {
         
            drop.position.bezierSize = dropped ? 1 : 0;
        }
    }
}
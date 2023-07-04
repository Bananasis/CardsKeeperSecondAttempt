namespace DefaultNamespace
{
    public abstract class CardDropZone:DropZone<RoomCardUI>
    {
        public virtual float dropVanish => 1;
        public virtual bool TakeControl(RoomCardUI card) =>false;
    }
}
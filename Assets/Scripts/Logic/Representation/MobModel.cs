using System.Collections.Generic;

public class MobModel : UnitModel<Mob>
{
    private RoomData _spawnRoom;

    public MobModel(Mob mob, DVector2 pos, RoomData spawnRoom) : base(mob, pos)
    {
        _movementStrategies.Add(new FearEntrance());
        _movementStrategies.Add(new Aggressive());
        _movementStrategies.Add(new GoToRoom());

        _spawnRoom = spawnRoom;
    }


    public override UnitType type => UnitType.Mob;

    public override void Plan(RoomAccessible neighbours, List<int> random,
        MultiValueDictionary<DVector2, UnitModel> plannedMovement)
    {
        GetPlan(neighbours, random, plannedMovement, _spawnRoom);
    }

    public override PreviewData GetPreviewData()
    {
        return new PreviewData() {sprite = unit.sprite};
    }
}
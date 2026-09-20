using UnityEngine;

public class MobileUnitCommand : UnitCommandBase
{
    public override UnitCommandType CommandType { get; set; } = UnitCommandType.Move;
    public Vector3 Location;

    public MobileUnitCommand(Vector3 location) {
        Location = location;
    }
}


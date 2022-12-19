using UnityEngine;

public class MobileUnitCommand : UnitCommandBase
{
    public new readonly UnitCommandType CommandType = UnitCommandType.Move;
    public Vector3 Location;

    public MobileUnitCommand(Vector3 location) {
        Location = location;
    }
}


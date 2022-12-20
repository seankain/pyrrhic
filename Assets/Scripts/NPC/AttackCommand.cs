using UnityEngine;

public class AttackCommand : UnitCommandBase
{
    public GameObject Target;
    public new readonly UnitCommandType CommandType = UnitCommandType.Attack;
    public AttackCommand(GameObject target)
    {
        Target = target;
    }
}


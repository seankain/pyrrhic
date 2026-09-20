using UnityEngine;

public class AttackCommand : UnitCommandBase
{
    public GameObject Target;
    public override UnitCommandType CommandType { get;  set; } =  UnitCommandType.Attack;
    public AttackCommand(GameObject target)
    {
        CommandType = UnitCommandType.Attack;
        Target = target;
    }
}


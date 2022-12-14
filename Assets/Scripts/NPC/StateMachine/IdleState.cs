using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState<GroundMobileCapability>
{

    public IdleState(GroundMobileCapability capability) : base("Idle", capability) {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
    }

}
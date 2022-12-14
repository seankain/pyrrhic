using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : BaseState<GroundMobileCapability>
{
    public MovingState(GroundMobileCapability capability) : base("Moving", capability) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void UpdateLogic()
    {

    }
    public override void Exit()
    {
        base.Exit();
    }
}
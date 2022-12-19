using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : BaseState<GroundMobileCapability>
{
    public MovingState(MobileUnitStateMachine stateMachine, GroundMobileCapability capability) : base("Moving",stateMachine, capability) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entered moving state");
    }

    public override void UpdateLogic()
    {
        Debug.Log($"{name} Moving state {capability.Commands.Count} commands in queue");
        if(capability.Commands.Count > 0)
        {
            
            var command = (MobileUnitCommand)capability.Commands.Pop();
            capability.NavigateTo(command.Location);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}
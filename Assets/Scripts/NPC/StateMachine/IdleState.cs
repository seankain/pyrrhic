using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState<GroundMobileCapability>
{
    private float gasTime = 15;
    private float elapsedTime = 0;

    public IdleState(MobileUnitStateMachine stateMachine, GroundMobileCapability capability) : base("Idle", stateMachine, capability) {
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        Debug.Log("Entered idle state");
        base.Enter();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= gasTime)
        {
            if(UnityEngine.Random.value > 0.5)
            {
                Debug.Log("fart sound here");
            }
            elapsedTime = 0;
        }
        Debug.Log($"Idle state: command count {capability.Commands.Count}");
        if (capability.Commands.Count >0)
        {
            stateMachine.ChangeState(((MobileUnitStateMachine)stateMachine).movingState);
        }
    }

}
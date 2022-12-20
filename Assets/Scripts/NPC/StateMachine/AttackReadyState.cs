using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackReadyState : BaseState<FightingUnitCapability>
{
    private float gasTime = 15;
    private float elapsedTime = 0;

    public AttackReadyState(FightingUnitStateMachine stateMachine, FightingUnitCapability capability) : base("AttackReady", stateMachine, capability)
    {
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        Debug.Log("Entered attack ready state");
        base.Enter();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (capability.Commands.Count > 0)
        {
            stateMachine.ChangeState(((FightingUnitStateMachine)stateMachine).Attacking);
        }
    }
}

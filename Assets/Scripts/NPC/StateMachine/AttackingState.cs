using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : BaseState<FightingUnitCapability>
{
    public AttackingState(FightingUnitStateMachine stateMachine, FightingUnitCapability capability) : base("Attacking", stateMachine, capability)
    {
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        Debug.Log("Entered fighting state");
        base.Enter();
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (capability.Commands.Count > 0)
        {
            var command = (AttackCommand)capability.Commands.Pop();
            var player = command.Target.GetComponent<PyrrhicPlayer>();
            if (player != null)
            {
                while (player.IsDead.Value == false)
                {
                    capability.Attack(command.Target);
                }
            }

        }
    }
}

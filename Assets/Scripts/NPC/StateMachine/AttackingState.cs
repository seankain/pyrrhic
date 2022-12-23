using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : BaseState<FightingUnitCapability>
{
    private PyrrhicPlayer currentTarget;
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
                currentTarget = player;
            }
        }
        //var command = (AttackCommand)capability.Commands.Pop();
        //var player = command.Target.GetComponent<PyrrhicPlayer>();


        // while (player.IsDead.Value == false)
        //  {
        capability.Attack(currentTarget.gameObject);
        // }
        //TODO use weapon range
        if (Vector3.Distance(currentTarget.transform.position, capability.gameObject.transform.position) > 20f)
        {
            currentTarget = null;
            stateMachine.ChangeState(((FightingUnitStateMachine)stateMachine).Ready);
        }
        if (currentTarget.IsDead.Value == true)
        {
            var otherPlayers = capability.GetPlayers();
            foreach (var otherPlayer in otherPlayers)
            {
                //TODO: make distance variable
                if (Vector3.Distance(otherPlayer.transform.position, currentTarget.transform.position) < 10f)
                {
                    currentTarget = otherPlayer;
                }
            }
            stateMachine.ChangeState(((FightingUnitStateMachine)stateMachine).Ready);
        }



    }
}

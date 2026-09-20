using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ReconState : BaseState<GroundMobileCapability>
{
    public ReconState(MobileUnitStateMachine stateMachine, GroundMobileCapability capability) : base("Recon", stateMachine, capability) { }

    public override void Enter()
    {
        Debug.Log("Entered recon state");
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

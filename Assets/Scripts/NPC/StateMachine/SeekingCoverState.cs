using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SeekingCoverState : BaseState<GroundMobileCapability>
{

    public SeekingCoverState(MobileUnitStateMachine stateMachine, GroundMobileCapability capability) : base("SeekingCover", stateMachine, capability)
    {

    }

}


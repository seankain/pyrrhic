using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : BaseState<GroundMobileCapability>
{
    public MovingState(GroundMobileCapability capability) : base("Moving", capability) { }
}
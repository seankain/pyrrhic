using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobileUnitStateMachine : StateMachine
{
    [HideInInspector]
    public IdleState idleState;
    [HideInInspector]
    public MovingState movingState;

    private GroundMobileCapability capability;

    private void Awake()
    {
        idleState = new IdleState(this);
        movingState = new MovingState(this);
        capability = GetComponent<GroundMobileCapability>();
    }

    protected override BaseState GetInitialState()
    {
        return idleState;
    }
}

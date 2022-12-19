using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobileUnitStateMachine : StateMachine<GroundMobileCapability>
{
    [HideInInspector]
    public IdleState idleState;
    [HideInInspector]
    public MovingState movingState;
    [HideInInspector]
    public ReconState reconState;

    private GroundMobileCapability capability;

    private void Awake()
    {
        capability = GetComponent<GroundMobileCapability>();
        idleState = new IdleState(this,capability);
        movingState = new MovingState(this,capability);
        reconState = new ReconState(this,capability);
        //idleState.StateExited += () => { movingState.Enter(); };
        //movingState.StateExited += () => { idleState.Enter(); };

    }

    protected override BaseState<GroundMobileCapability> GetInitialState()
    {
        return idleState;
    }
}

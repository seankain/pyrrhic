using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FightingUnitStateMachine : StateMachine<FightingUnitCapability>
{
    [HideInInspector]
    public AttackingState Attacking;
    [HideInInspector]
    public AttackReadyState Ready;

    private FightingUnitCapability capability;

    private void Awake()
    {
        capability = GetComponent<FightingUnitCapability>();
        Attacking = new AttackingState(this, capability);
        Ready = new AttackReadyState(this, capability);
    }

    protected override BaseState<FightingUnitCapability> GetInitialState()
    {
        return Ready;
    }
}
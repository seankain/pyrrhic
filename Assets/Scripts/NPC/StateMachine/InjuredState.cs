using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjuredState : BaseState<Damageable>
{
    public InjuredState(StateMachine<Damageable> stateMachine,Damageable capability) : base("Injured",stateMachine, capability) { }
}
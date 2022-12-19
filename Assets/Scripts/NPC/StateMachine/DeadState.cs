using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : BaseState<Damageable>
{
    public DeadState(StateMachine<Damageable> stateMachine, Damageable capability) : base("Dead", stateMachine, capability) { }
}
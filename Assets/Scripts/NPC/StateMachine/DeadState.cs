using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : BaseState<Damageable>
{
    public DeadState(Damageable capability) : base("Dead", capability) { }
}
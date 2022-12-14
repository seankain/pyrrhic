using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjuredState : BaseState<Damageable>
{
    public InjuredState(Damageable capability) : base("Injured", capability) { }
}
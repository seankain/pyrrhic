using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//Not well thought out yet but I need it for the state machine

public class Damageable : UnitCapability
{
    
    public float HitPoints = 100f;

    public void Injure() { }
    public void Die() { }
}


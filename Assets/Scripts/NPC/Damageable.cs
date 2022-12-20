using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


//Not well thought out yet but I need it for the state machine

public class Damageable : UnitCapability
{
    
    public float HitPoints = 100f;

    public void Injure(float damagePoints) { 
        HitPoints -= damagePoints;
        if(HitPoints <= 0)
        {
            Die();
        }
    }
    public void Die() 
    {
        Debug.Log("An implementation of die is needed here");
    }

    protected override void OnStarting()
    {

    }
}


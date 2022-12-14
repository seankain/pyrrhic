using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState<T> where T : UnitCapability
{
    public delegate void StateExitedDelegate();
    public event StateExitedDelegate StateExited;
    public delegate void StateEnteredDelegate();
    public event StateEnteredDelegate StateEntered;

    public string name;

    protected T capability;

    public BaseState(string name, T capability)
    {
        this.name = name;
        this.capability = capability;
    }

    public virtual void Enter()
    {
        if (StateEntered != null)
        {
            StateEntered.Invoke();
        }
    }
    public virtual void UpdateLogic() { }
    public virtual void UpdatePhysics() { }
    public virtual void Exit()
    {
        if (StateExited != null)
        {
            StateExited.Invoke();
        }
    }
}

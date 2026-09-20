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
    protected StateMachine<T> stateMachine;

    public BaseState(string name, StateMachine<T> stateMachine, T capability)
    {
        this.name = name;
        this.capability = capability;
        this.stateMachine = stateMachine;
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

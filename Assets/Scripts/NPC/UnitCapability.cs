using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitCapability : MonoBehaviour
{
    protected Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        OnStarting();
    }

    protected abstract void OnStarting();
}

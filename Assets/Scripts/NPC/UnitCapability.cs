using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitCapability : MonoBehaviour
{
    protected Animator anim;
    protected UnitBase unitBase;
    void Start()
    {
        anim = GetComponent<Animator>();
        unitBase = GetComponent<UnitBase>();
        OnStarting();
    }

    public PyrrhicPlayer[] GetPlayers()
    {
        return FindObjectsOfType<PyrrhicPlayer>();
    }

    protected abstract void OnStarting();
}

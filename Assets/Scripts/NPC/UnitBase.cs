using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    public string UnitName = "Unit";
    public UnitCapability[] Capabilities;
    public bool Destructable = true;
    public float HitPoints = 100f;
    public bool CanAttack = true;   
    public bool IsTransport = false;
    public bool IsStationary = false;
    //TODO: replace with NpcBase networked
    protected NpcBaseDebug npcBase;


    // Start is called before the first frame update
    void Start()
    {
        npcBase = GetComponent<NpcBaseDebug>();
        Capabilities = GetComponents<UnitCapability>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

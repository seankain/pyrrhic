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
        // TODO add capability components at runtime based on unit base properties instead of adding them here
        if (!IsStationary && !IsTransport)
        {
            gameObject.AddComponent<GroundMobileCapability>();
        }
        if (Destructable)
        {
            var damageable = gameObject.AddComponent<Damageable>();
            damageable.HitPoints = HitPoints;
        }
        if (CanAttack)
        {
            gameObject.AddComponent<FightingUnitCapability>();
            //TODO: configure weapons here
        }
        Capabilities = GetComponents<UnitCapability>();
    }

    // Update is called once per frame
    void Update()
    {

    }


}

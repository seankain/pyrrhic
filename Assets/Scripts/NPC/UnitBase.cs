using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    public string UnitName = "Unit";
    [HideInInspector]
    public UnitCapability[] Capabilities;
    [HideInInspector]
    public CommandableCapability[] CommandableCapabilities;
    public bool Destructable = true;
    public float HitPoints = 100f;
    public bool CanAttack = true;
    public bool IsTransport = false;
    public bool IsStationary = false;
    public NpcWeaponData[] WeaponLoadouts;
    [HideInInspector]
    public List<NpcWeapon> Weapons;

    // Start is called before the first frame update
    void Start()
    {
        //npcBase = GetComponent<NpcBaseDebug>();
        // TODO add capability components at runtime based on unit base properties instead of adding them here
        if (!IsStationary && !IsTransport)
        {
            gameObject.AddComponent<GroundMobileCapability>();
            gameObject.AddComponent<MobileUnitStateMachine>();
        }
        if (Destructable)
        {
            var damageable = gameObject.AddComponent<Damageable>();
            damageable.HitPoints = HitPoints;
        }
        if (CanAttack)
        {
            var fighting = gameObject.AddComponent<FightingUnitCapability>();
            //TODO: configure weapons here
            foreach (var weaponData in WeaponLoadouts) 
            {
                fighting.GiveWeapon(weaponData);
            }
        }
        Capabilities = GetComponents<UnitCapability>();
        CommandableCapabilities = GetComponents<CommandableCapability>();
    }

    public void AddCommand(UnitCommandBase unitCommand)
    {
        //TODO: parse command types and send to the right place
        foreach(var capability in CommandableCapabilities)
        {
            if(capability.CapabilityCommand == unitCommand.CommandType)
            {
                capability.Commands.Push(unitCommand);
            }
        }
    }


    // Update is called once per frame
    void Update()
    {

    }


}

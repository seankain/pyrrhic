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
        if (!IsStationary && !IsTransport)
        {
            gameObject.AddComponent<GroundMobileCapability>();
            gameObject.AddComponent<MobileUnitStateMachine>();
        }
        if (Destructable)
        {
            var damageable = gameObject.AddComponent<Damageable>();
        }
        if (CanAttack)
        {
            var fighting = gameObject.AddComponent<FightingUnitCapability>();
            //TODO: configure weapons here
            foreach (var weaponData in WeaponLoadouts) 
            {
                fighting.GiveWeapon(weaponData);
            }
            gameObject.AddComponent<FightingUnitStateMachine>();
        }
        Capabilities = GetComponents<UnitCapability>();
        CommandableCapabilities = GetComponents<CommandableCapability>();
    }

    public void AddCommand(UnitCommandBase unitCommand)
    {
        //TODO: parse command types and send to the right place
        foreach(var capability in CommandableCapabilities)
        {
            if(capability.CapabilityCommand.Equals(unitCommand.CommandType))
            {
                capability.Commands.Push(unitCommand);
            }
        }
    }

    public bool CanBeSeenByPlayer()
    {
        var players = FindObjectsOfType<PyrrhicPlayer>();
        foreach(var player in players)
        {
            var angle = -Vector3.SignedAngle((player.transform.position - transform.position), player.transform.forward, Vector3.up);
            if(angle > -90 && angle < 90)
            {
                return true;
            }
        }
        return false;
    }

    public PyrrhicPlayer[] GetPlayers()
    {
        return FindObjectsOfType<PyrrhicPlayer>();
    }

    public void HandleHit(float damage)
    {

    }


    // Update is called once per frame
    void Update()
    {

    }


}

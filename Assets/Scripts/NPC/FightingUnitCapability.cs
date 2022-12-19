using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FightingUnitCapability : CommandableCapability
{
    /// <summary>
    /// Dictates how often unit prioritizes attacking over other behaviors
    /// 0 is passive area defense
    /// 100 is doggedly chasing players with disregard for personal status
    /// </summary>
    [Range(0, 100f)]
    public float Aggression = 50;
    /// <summary>
    /// Dictates how close the unit will get to enemies by preference
    /// 0 is melee 
    /// 100 is maximum map/weapon range
    /// </summary>
    [Range(0, 100f)]
    public float Range = 50;

    /// <summary>
    /// The distance the unit can attack with melee
    /// </summary>
    public float MeleeRange = 1.5f;

    private EquipSockets equipSockets;

    [SerializeField]
    private AudioSource attackSound;
    //private Animator anim;
    //private NpcBaseDebug npcBase;
    private bool attacking = false;
    private float attackCooldownElapsed = 0f;
    private float attackCooldownSeconds = 1.0f;
    private GameObject weapon;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    void Awake()
    {
        equipSockets = GetComponent<EquipSockets>();
    }

    protected override void OnStarting()
    {
        CapabilityCommand = UnitCommandType.Attack;

        //anim = GetComponent<Animator>();
        //npcBase = GetComponent<NpcBaseDebug>();
    }

    // Update is called once per frame
    void Update()
    {
        //var playersInRange = npcBase.GetPlayersInRange();
        //if (playersInRange.Count > 0 && attacking == false)
        //{
        //    Attack();
        //}
        //if (attacking)
        //{
        //    attackCooldownElapsed += Time.deltaTime;
        //    if (attackCooldownElapsed >= attackCooldownSeconds)
        //    {
        //        attacking = false;
        //    }
        //}
    }

    public void GiveWeapon(NpcWeaponData data)
    {
        weapon = Instantiate(data.Model, equipSockets.FrontHoldPosition.position, equipSockets.FrontHoldPosition.rotation, equipSockets.FrontHoldPosition);
    }

    public void Attack(GameObject gameObject)
    {
        transform.LookAt(gameObject.transform);
        attacking = true;
        anim.SetBool("Shooting", true);
        //var projectile = Instantiate(projectilePrefab, firePosition.transform.position, Quaternion.identity, null);
        //projectile.GetComponent<NetworkObject>().Spawn();
        //var pyrProjectile = projectile.GetComponent<PyrProjectile>();
        //pyrProjectile.Damage = 20;
        //pyrProjectile.Direction = firePosition.forward;
        //pyrProjectile.Send();
        attackSound.Play();
    }

}

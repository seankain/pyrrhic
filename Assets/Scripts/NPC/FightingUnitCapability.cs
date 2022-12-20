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
    private NpcWeapon weapon;
    private FightingUnitStateMachine stateMachine;
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
        stateMachine = GetComponent<FightingUnitStateMachine>();
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
        var weaponObj = Instantiate(data.Model, equipSockets.FrontHoldPosition.position, equipSockets.FrontHoldPosition.rotation, equipSockets.FrontHoldPosition);
        var audioSource = weaponObj.AddComponent<AudioSource>();
        audioSource.clip = data.FireSound;
        weapon = weaponObj.AddComponent<NpcWeapon>();
        weapon.FireAudio = audioSource;
        weapon.FirePosition = weaponObj.transform.Find("Muzzle");
        //var projectilePrefab = new GameObject();
        var projectilePrefab = data.Ammunition.Model;
        var projectile = projectilePrefab.GetComponent<PyrProjectile>();
        projectile.Damage = data.Ammunition.Damage;
        projectile.MassGrams = data.Ammunition.ProjectileMassGrams;
        projectile.SpeedMetersPerSecond = data.Ammunition.MuzzleVelocity;
        weapon.ProjectilePrefab = projectile;
    }

    public void Attack(GameObject gameObject)
    {
        transform.LookAt(gameObject.transform);
        attacking = true;
        anim.SetBool("Shooting", true);
        weapon.Fire();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        var gripPos = weapon.transform.Find("GripIKHint");
        var foregripPos = weapon.transform.Find("ForegripIKHint");
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.RightHand, gripPos.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, gripPos.rotation);

        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, foregripPos.position);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, foregripPos.rotation);


    }

}

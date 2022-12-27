using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FPPistol : MonoBehaviour
{
    [SerializeField]
    public float Damage = 30f;
    public Vector3 IronSightPosition;
    private PyrrhicPlayer owner;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private AudioSource fireAudioSource;
    [SerializeField]
    private AudioSource reloadAudioSource;
    [SerializeField]
    private GameObject magazinePrefab;
    [SerializeField]
    private GameObject casingPrefab;
    [SerializeField]
    private GameObject muzzleFlashPrefab;
    [SerializeField]
    private GameObject muzzleFlashPoint;
    [SerializeField]
    private Transform caseEjectPoint;
    [SerializeField]
    private int capacity = 15;
    private int currentRounds = 0;
    [SerializeField]
    private float fireDelaySeconds = 0.1f;
    [SerializeField]
    private float reloadTime = 2.0f;
    [SerializeField]
    private GameObject ProjectilePrefab;


    private bool isFiring = false;
    private float fireElapsed = 0;

    private bool isReloading = false;
    private float reloadingElapsed = 0;
    private bool triggerDepressed = false;
    private bool triggerSet = true;

    private List<GameObject> casingPrefabPool = new List<GameObject>();


    public void Fire()
    {
        if (isFiring || triggerSet == false) { return; }
        if (currentRounds == 0) { anim.SetBool("Empty", true); return; }
        isFiring = true;
        if (triggerDepressed)
        {
            triggerSet = false;
        }
        SpawnProjectile();
        anim.Play("Fire");
        fireAudioSource.Play();
        
        currentRounds--;

    }

    public void ToggleDownSight(bool downSight)
    { 
        
    }

    public void Draw()
    {
        anim.Play("Draw");
    }

    public void Stow()
    {
        anim.Play("Stow");

    }

    public void Reload()
    {
        if (currentRounds == capacity) { return; }
        if (!isReloading)
        {
            isReloading = true;
            anim.SetBool("Empty", false);
            if (currentRounds == 0)
            {
                anim.Play("ReloadFull");
            }
            if (currentRounds >= 1)
            {
                anim.Play("Reload");
            }
            currentRounds = capacity;
            reloadAudioSource.Play();
        }
    }

    private void SpawnProjectile()
    {
        //TODO the prefab I'm using for testing is rotated and needs to be set to match forward
        var pref = Instantiate(ProjectilePrefab,muzzleFlashPoint.transform.position,Quaternion.Euler(90,0,0),null);
        var projectile = pref.GetComponent<PyrProjectile>();
        pref.GetComponent<NetworkObject>().Spawn(true);
        //TODO get owner id if that is important
        projectile.ownerId = owner.OwnerClientId;
        projectile.Damage = Damage;
        projectile.Direction = muzzleFlashPoint.transform.forward;
        projectile.Send();
    }

    // Start is called before the first frame update
    void Start()
    {
        owner = gameObject.GetComponentInParent<PyrrhicPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        var fire1 = Input.GetAxis("Fire1");
        //Clumsily trying to inform semi/full auto weapons about finger/trigger state TODO: revist
        if (fire1 == 1 && triggerDepressed == false)
        {
            triggerDepressed = true;
        }
        if (fire1 == 0 && triggerDepressed == true)
        {
            triggerDepressed = false;
            triggerSet = true;
        }
        //Theyre holding it down
        if (fire1 == 1 && triggerDepressed == true) {
            triggerDepressed = true;
            triggerSet = false;
        }

        if (isFiring)
        {
            //Maybe a coroutine that checks animation/sound state would be better
            fireElapsed += Time.deltaTime;
            if (fireElapsed >= fireDelaySeconds)
            {
                isFiring = false;
                fireElapsed = 0;
            }
        }
        if (isReloading)
        {
            reloadingElapsed += Time.deltaTime;
            if (reloadingElapsed >= reloadTime)
            {
                isReloading = false;
                reloadingElapsed = 0;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PyrProjectileOffline : MonoBehaviour
{
    public ProjectileInfo Info;
    public float Damage = 30f;
    private Rigidbody rb;
    public ulong ownerId;
    public Vector3 currentVelocity;
    public List<GameObject> CurrentContacts { get { return currentContacts; } }
    private List<GameObject> currentContacts = new List<GameObject>();
    [SerializeField]
    private float maxLifetimeSeconds = 30;
    public float elapsed = 0;

    private void Awake()
    {
        //rb = gameObject.GetComponent<Rigidbody>();

    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Send()
    {
        //rb.AddForce(Direction * (MassGrams * (SpeedMetersPerSecond*SpeedMetersPerSecond)), ForceMode.Impulse);
        //rb.AddForce(Direction * SpeedMetersPerSecond, ForceMode.VelocityChange);
        StartCoroutine(FlightCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        //elapsed += Time.deltaTime;
        //if (elapsed >= maxLifetimeSeconds)
        //{
        //    //TODO look into projectile pooling
        //    Destroy(gameObject);
        //    //NetworkServer.Destroy(gameObject);
        //}
    }

    private IEnumerator FlightCoroutine()
    {
        currentVelocity = transform.forward * Info.MuzzleVelocity;
        while (elapsed < maxLifetimeSeconds)
        {
            elapsed += Time.deltaTime;
            IntegrationMethods.Heuns(elapsed, transform.position, currentVelocity, Vector3.zero,
                new BulletData
                {
                    muzzleVelocity = Info.MuzzleVelocity,
                    m = Info.MassGrams / 1000,
                    C_d = Info.BallisticCoefficientG1,
                    r = Info.RadiusMillimeters / 1000,
                    rho = 1.204f
                },
                out var nextPosition,
                out var nextVelocity,
                out var angleDelta);
            currentVelocity = nextVelocity;
            if (Physics.Raycast(transform.position, transform.forward, out var hit, Vector3.Distance(transform.position, nextPosition), ~0, QueryTriggerInteraction.Ignore))
            {

                Debug.Log($"hit {hit.collider.name} after {elapsed} seconds");
                var hittable = hit.collider.gameObject.GetComponent<IHittable>();
                if (hittable != null)
                {
                    hittable.HandleHit(Damage, hit.point);
                }
                transform.position = nextPosition;
                yield break;
            }
            transform.position = nextPosition;
            yield return null;
        }

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log($"Whacked {collision.collider.gameObject.name}");
    //    currentContacts.Add(collision.contacts[0].otherCollider.gameObject);
    //    var hittable = collision.collider.gameObject.GetComponent<IHittable>();
    //    if (hittable != null)
    //    {
    //        Debug.Log("Hit a hittable");
    //    }
    //}
}


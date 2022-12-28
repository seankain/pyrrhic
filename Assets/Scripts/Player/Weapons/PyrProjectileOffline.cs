using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PyrProjectileOffline : MonoBehaviour
{
    public float Damage = 30f;
    private Rigidbody rb;
    public ulong ownerId;
    public float SpeedMetersPerSecond = 300;
    public List<GameObject> CurrentContacts { get { return currentContacts; } }
    private List<GameObject> currentContacts = new List<GameObject>();
    [SerializeField]
    public float MassGrams = 0.1f;
    [SerializeField]
    public Vector3 Direction = Vector3.forward;
    [SerializeField]
    private float forceMultiplier = 10000;
    [SerializeField]
    private float maxLifetimeSeconds = 30;
    private float elapsed = 0;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Send()
    {
        //rb.AddForce(Direction * (MassGrams * (SpeedMetersPerSecond*SpeedMetersPerSecond)), ForceMode.Impulse);
        rb.AddForce(Direction * SpeedMetersPerSecond, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= maxLifetimeSeconds)
        {
            //TODO look into projectile pooling
            Destroy(gameObject);
            //NetworkServer.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Whacked {collision.collider.gameObject.name}");
        currentContacts.Add(collision.contacts[0].otherCollider.gameObject);
        var hittable = collision.collider.gameObject.GetComponent<IHittable>();
        if (hittable != null)
        {
            Debug.Log("Hit a hittable");
        }
    }
}

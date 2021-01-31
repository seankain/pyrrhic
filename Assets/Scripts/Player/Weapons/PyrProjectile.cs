using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PyrProjectile : NetworkBehaviour
{
    public float Damage = 30f;
    private Rigidbody rb;
    public uint ownerId;
    [SerializeField]
    private float SpeedMetersPerSecond = 300;
    [SerializeField]
    private float mass = 0.1f;
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

        rb.AddForce(Direction * forceMultiplier, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= maxLifetimeSeconds)
        {
            NetworkServer.Destroy(gameObject);
        }
    }

    [Command]
    void HandlePlayerHit(PyrrhicPlayer player)
    {
        player.HandleHit(player.connectionToClient, Damage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Whacked {collision.collider.gameObject.name}");
        var player = collision.collider.gameObject.GetComponent<PyrrhicPlayer>();
        if (player != null)
        {
            HandlePlayerHit(player);
            NetworkServer.Destroy(gameObject);
            return;
        }
        var enemy = collision.collider.gameObject.GetComponent<EnemyBase>();
        if(enemy != null)
        {
            enemy.HandleHit(Damage);
        }

    }
}

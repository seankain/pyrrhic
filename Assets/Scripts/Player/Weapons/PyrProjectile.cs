using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PyrProjectile : NetworkBehaviour
{
    public float Damage = 30f;
    private Rigidbody rb;
    public ulong ownerId;
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
            //TODO look into projectile pooling
            Destroy(gameObject);
            //NetworkServer.Destroy(gameObject);
        }
    }

    [ServerRpc]
    void HandlePlayerHitServerRpc(ulong ownerClientId)
    {
        var players = FindObjectsOfType<PyrrhicPlayer>();
        foreach (var player in players)
        {
            //TODO could use a sublist of client ids in clientrpc to avoid sending to everyone and checking ids a bunch
            if (player.OwnerClientId == ownerClientId)
            {
                player.HandleHitClientRpc(ownerClientId, Damage);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Whacked {collision.collider.gameObject.name}");
        var player = collision.collider.gameObject.GetComponent<PyrrhicPlayer>();
        if (player != null)
        {
            HandlePlayerHitServerRpc(player.OwnerClientId);
            //TODO use a pool or something else
            Destroy(gameObject);
            //NetworkServer.Destroy(gameObject);
            return;
        }
        var enemy = collision.collider.gameObject.GetComponent<EnemyBase>();
        if(enemy != null)
        {
            enemy.HandleHit(Damage);
        }

    }
}

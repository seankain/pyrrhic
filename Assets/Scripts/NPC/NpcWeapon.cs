using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NpcWeapon : NetworkBehaviour
{
    [SerializeField]
    private GameObject ProjectilePrefab;
    [SerializeField]
    private float FireDelaySeconds = 0.3f;
    [SerializeField]
    private float Damage = 20f;
    [SerializeField]
    private Transform MuzzleTransform;
    private float elapsed = 0;
    private bool canFire = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed >= FireDelaySeconds)
        {
            elapsed = 0;
            canFire = true;
        }
    }

    public void Fire()
    {
        canFire = false;
    }

}

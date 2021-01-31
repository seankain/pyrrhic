using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBase : NetworkBehaviour
{
    [SyncVar]
    public float Health = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ClientRpc]
    private void HandleDamage() { }

    [ClientRpc]
    private void Die()
    {
        Debug.Log($"{gameObject.name} Died");
        Destroy(this);
    }

    [Command]
    public void ProcessHit()
    {
        Die();
    }

}

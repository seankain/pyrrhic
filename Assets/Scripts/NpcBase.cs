using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NpcBase : NetworkBehaviour
{

    public NetworkVariable<float> Health = new NetworkVariable<float>(100);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ClientRpc]
    private void HandleDamageClientRpc() { }

    [ClientRpc]
    private void DieClientRpc()
    {
        Debug.Log($"{gameObject.name} Died");
        Destroy(this);
    }

    [ServerRpc]
    public void ProcessHitServerRpc()
    {
        DieClientRpc();
    }

}

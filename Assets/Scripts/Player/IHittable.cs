using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public interface IHittable
{
    public void HandleHit(float damage, Vector3 location);
}

public interface INetworkHittable
{
    [ClientRpc]
    public void HandleHitClientRpc();
}


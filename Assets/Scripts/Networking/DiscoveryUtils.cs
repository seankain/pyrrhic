using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct DiscoveryRequest /*Is there a netcode equivalent: NetworkMessage*/
{
   
}
public struct DiscoveryResponse /* Is there a netcode equivalent: NetworkMessage*/
{
    // you probably want uri so clients know how to connect to the server
    public Uri ServerUri;
   
    public int TotalPlayers;
    public int CurrentBoots;
    public int CurrentStrategists;
    public int MaxPlayers;
    public string ServerName;

    // Add properties for whatever information you want the server to return to
    // clients for them to display or consume for establishing a connection.
}

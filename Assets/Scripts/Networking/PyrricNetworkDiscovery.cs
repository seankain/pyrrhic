using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Net;

public class PyrricNetworkDiscovery /*: NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>*/
{
    //public delegate void HostFoundDelegate(DiscoveryResponse response);
    ////public delegate void StartedServerBroadcasDelegate();
    //public event HostFoundDelegate HostFound;
    ////public event StartedServerBroadcasDelegate StartedAsServer;
    ////public PyrricNetworkManager NetMgr;
    ////private bool Seeking = true;
    ////private bool SwitchedBroadcast = false;
    ////public float PoolWaitTime = 10.0f;
    ////private float Elapsed = 0;
    ////private string foundIP;
    //////Doesnt seem to be in Mirror
    //////public DiscoveryHostFound 
    ////private void Awake()
    ////{
    ////    //UnityDiscoveryHostFound = new DiscoveryHostFound();
    ////}

    ////private void Start()
    ////{
    ////    //Not in mirror
    ////    //NetworkTransport.Init();
    ////    DontDestroyOnLoad(gameObject);
    ////    //showGUI = false;
    ////    //StartAsClient();
    ////    StartDiscovery();
    ////    this.OnServerFound.AddListener(HandleServerFound);
    ////}

    ////private void HandleServerFound(ServerResponse response)
    ////{
    ////    Debug.Log(response);
    ////}

    //private PyrServerInfo LocalServerInfo;

    //public override void Start()
    //{
    //    LocalServerInfo = GameObject.FindObjectOfType<PyrServerInfo>();
    //    base.Start();
    //}

    //protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
    //{
    //    return new DiscoveryResponse
    //    {
    //        ServerName = LocalServerInfo.ServerName,
    //        CurrentBoots = (int)LocalServerInfo.CurrentBootTeamSize,
    //        CurrentStrategists = (int)LocalServerInfo.CurrentStrategistTeamSize,
    //        MaxPlayers = (int)(LocalServerInfo.MaxBootTeamSize + LocalServerInfo.MaxStrategistTeamSize),
    //        TotalPlayers = (int)(LocalServerInfo.CurrentBootTeamSize + LocalServerInfo.CurrentStrategistTeamSize),
    //        ServerUri = new Uri(LocalServerInfo.ServerUri)
    //    };
    //}

    //protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    //{
    //   if(HostFound != null) { HostFound.Invoke(response); }
    //}

    //protected override void ProcessClientRequest(DiscoveryRequest request, IPEndPoint endpoint)
    //{
    //    base.ProcessClientRequest(request, endpoint);
    //}


    //protected override DiscoveryRequest GetRequest()
    //{
    //    return new DiscoveryRequest();
    //}

}

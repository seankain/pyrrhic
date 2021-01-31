using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DebugNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        Debug.Log("On Server start");
        
    }

    public override void OnStopServer() {
        Debug.Log("On Server stop");
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log($"Client connected {conn.connectionId}");
        ClientScene.AddPlayer(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log($"Client disconnected {conn.connectionId}");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //var spawnLocation = GameObject.FindGameObjectWithTag("TeamSelectSpawn");
        var player = Instantiate(playerPrefab, Vector3.up*4,Quaternion.identity,null);
        NetworkServer.AddPlayerForConnection(conn, player);
        player.GetComponent<PyrrhicPlayer>().PromptTeamSelect(conn);
       
    }

}

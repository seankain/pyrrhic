//using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Unity.Netcode;

public enum LocalGameState { Hosting,Menu,InHostedGame }
public class PyrServerInfo : NetworkBehaviour
{
    public LocalGameState CurrentState = LocalGameState.Menu;
    public uint CurrentPlayerCount = 0;
    public string ServerName = "Pyrrhic Firefight";
    public uint StrategistScore = 0;
    public uint BootScore = 0;
    public uint TimeRemainingSeconds = 1800;
    public uint CurrentBootTeamSize = 0;
    public uint CurrentStrategistTeamSize = 0;
    public uint MaxBootTeamSize = 4;
    public uint MaxStrategistTeamSize = 2;
    public string ServerUri = "127.0.0.1";
    public List<PyrrhicPlayer> Players;

    // Start is called before the first frame update
    void Start()
    {
        ServerUri = GetIP();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private string GetIP() {
        //From 'https://stackoverflow.com/questions/6803073/get-local-ip-address'
        // Thanks 'Mr Wang from Next Door'
        string localIP;
        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        {
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address.ToString();
        }
        return localIP;
    }
}

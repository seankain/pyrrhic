using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;

public struct TeammateInfo : INetworkSerializable
{
    public string Name;
    public int Score;
    public ulong ClientId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref Score);
        serializer.SerializeValue(ref ClientId);
    }
}

public struct TeamInfo : INetworkSerializable
{
    public List<TeammateInfo> Teammates;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        var teammates = Teammates.ToArray();
        var length = 0;
        if (serializer.IsWriter)
        {
            length = teammates.Length;
        }
        serializer.SerializeValue(ref length);
        for (var i = 0; i < length; i++)
        {
            serializer.SerializeValue(ref teammates[i]);
        }
    }

}
public class PyrrhicGame : NetworkBehaviour
{
    public NetworkVariable<int> BootTeamLivesRemaining = new NetworkVariable<int>();
    public NetworkVariable<int> StrategistTickets = new NetworkVariable<int>();
    public NetworkVariable<TeamInfo> BootTeamInfo = new NetworkVariable<TeamInfo>();
    public NetworkVariable<TeamInfo> StrategistTeamInfo = new NetworkVariable<TeamInfo>();
    public NetworkVariable<TeamInfo> Spectators = new NetworkVariable<TeamInfo>();

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        BootTeamInfo.Value = new TeamInfo { Teammates = new List<TeammateInfo>() };
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerToTeamServerRpc(ulong clientId, string playerName, PyrrhicTeam team)
    {
        if (team == PyrrhicTeam.Boot)
        {
            BootTeamInfo.Value.Teammates.Add(new TeammateInfo { Name = playerName, Score = 0 , ClientId = clientId});
        }
        if(team == PyrrhicTeam.Strategist)
        {
            StrategistTeamInfo.Value.Teammates.Add(new TeammateInfo { Name = playerName, Score = 0, ClientId = clientId }); ;
        }
    }

    private void Singleton_OnClientDisconnectCallback(ulong obj)
    {
        Debug.Log($"Client disconnected {obj}");
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        Debug.Log($"Client connected {obj}");
    }
}


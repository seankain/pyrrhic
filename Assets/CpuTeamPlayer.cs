using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CpuTeamPlayer : NetworkBehaviour
{

    public PyrrhicTeam Team = PyrrhicTeam.Strategist;
    public List<Vector3> SpawnLocations;
    public int MaximumTeammates = 5;
    public float SpawnFrequency = 10f;
    [SerializeField]
    private float CommandAssignmentFrequency = 5f;
    private List<float> assignmentsElapsed;
    public GameObject TeammatePrefab;
    private PyrServerInfo serverInfo;
    private float elapsed = 0f;
    private List<GameObject> teammates;
    private PyrrhicPlayer[] players;
    // Start is called before the first frame update
    void Start()
    {
        serverInfo = FindObjectOfType<PyrServerInfo>();
        teammates = new List<GameObject>();
        assignmentsElapsed = new List<float>();
        players = FindObjectsOfType<PyrrhicPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkManager.Singleton.IsListening || !IsServer) { return; }
        players = FindObjectsOfType<PyrrhicPlayer>();
        elapsed += Time.deltaTime;
        if (elapsed >= SpawnFrequency && teammates.Count < MaximumTeammates)
        {
            var teammate = Instantiate(TeammatePrefab, SpawnLocations[0], Quaternion.identity, null);
            teammate.GetComponent<NetworkObject>().Spawn();
            teammates.Add(teammate);
            assignmentsElapsed.Add(0);
            elapsed = 0;
        }
        for (var i = teammates.Count - 1; i >= 0; i--)
        {
            //teammate dead?
            if (teammates[i] == null)
            {
                teammates.RemoveAt(i);
                assignmentsElapsed.RemoveAt(i);
                continue;
            }
            //increment assignment elasped time
            assignmentsElapsed[i] += Time.deltaTime;
            if (assignmentsElapsed[i] >= CommandAssignmentFrequency)
            {
                //issue command
                assignmentsElapsed[i] = 0;
                var unitBase = teammates[i].GetComponent<UnitBase>();
                if (players.Length > 0)
                {
                    //TODO how to get the attack range
                    if (Vector3.Distance(players[0].transform.position, teammates[i].transform.position) < 20)
                    {
                        Debug.Log($"cpu player issuing attack command to {teammates[i].name} against {players[0].name}");
                        teammates[i].GetComponent<FightingUnitCapability>().Attack(players[0].gameObject);
                    }
                    else
                    {
                        Debug.Log($"cpu player issuing move command to {teammates[i].name}");
                        unitBase.AddCommand(new MobileUnitCommand(players[0].transform.position));
                    }
                }
            }


        }
    }
}

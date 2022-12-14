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
    public GameObject TeammatePrefab;
    private PyrServerInfo serverInfo;
    private float elapsed = 0f;
    private List<GameObject> teammates;

    // Start is called before the first frame update
    void Start()
    {
        serverInfo = FindObjectOfType<PyrServerInfo>();
        teammates = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!NetworkManager.Singleton.IsListening) { return; }
        elapsed += Time.deltaTime;
        if(elapsed >= SpawnFrequency && teammates.Count < MaximumTeammates)
        {
            var teammate = Instantiate(TeammatePrefab, SpawnLocations[0], Quaternion.identity, null);
            teammate.GetComponent<NetworkObject>().Spawn();
            teammates.Add(teammate);
        }
        foreach(var teammate in teammates)
        {
            var mobile = teammate.GetComponent<GroundMobileCapability>();
            if(mobile != null && !mobile.Moving)
            {
                mobile.SeekCloseSafe();
            }
        }
    }
}

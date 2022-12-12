using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GroundMobileCapability : UnitCapability
{
    public bool Moving => !navMeshAgent.isStopped;
    private NavMeshAgent navMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void NavigateTo(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
    }

    public void SeekCloseSafe()
    {
       var randomLocation =  Random.insideUnitSphere * 3f;
       navMeshAgent.SetDestination(gameObject.transform.position + new Vector3(randomLocation.x,0,randomLocation.z));
     
    }

}

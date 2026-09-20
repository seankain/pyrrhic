using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GroundMobileCapability : CommandableCapability
{
    public bool Moving => !navMeshAgent.isStopped;
    public bool HasActiveDestination => navMeshAgent.destination != null;
    public float MaxSpeed = 1f;
    //public Stack<MobileUnitCommand> Commands;
    private NavMeshAgent navMeshAgent;
    private MobileUnitStateMachine stateMachine;

    protected override void OnStarting()
    {
        CapabilityCommand = UnitCommandType.Move;
        //Commands = new Stack<UnitCommandBase>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<MobileUnitStateMachine>();
    }
    // Update is called once per frame
    void Update()
    {
        if(anim == null)
        {
            Debug.LogError("animator is null!");
        }
        navMeshAgent.speed = MaxSpeed;
        anim.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }
    public void NavigateTo(Vector3 destination)
    {
        Debug.Log($"mobile unit attempting to go to {destination}");
        navMeshAgent.SetDestination(destination);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(navMeshAgent.destination, 3);
    }

    public void SeekCloseSafe()
    {
        var randomLocation = Random.insideUnitSphere * 3f;
        navMeshAgent.SetDestination(gameObject.transform.position + new Vector3(randomLocation.x, 0, randomLocation.z));

    }

}

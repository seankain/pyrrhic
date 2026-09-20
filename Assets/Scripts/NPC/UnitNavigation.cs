using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A centralized wrapper to communicate with the navigation mesh agent. The units prioritize commanded locations from humans but after that will have 
/// autonomous navmesh destinations such as cover seeking or maneuvering on enemies i.e. not just standing there waiting to be shot if not being commanded
/// </summary>
public class UnitNavigation : MonoBehaviour
{
    public NavMeshAgent NavAgent;
    public Stack<Vector3> CommandedLocations = new Stack<Vector3>();
    public Stack<Vector3> AutonomousLocations = new Stack<Vector3>();

    public void Start()
    {
        NavAgent.GetComponent<NavMeshAgent>();
    }

    public void Update()
    {
        //Already going somewhere
        if (NavAgent.hasPath)
        {
            return;
        }
        if (CommandedLocations.Count > 0)
        {
            var nextLocation = CommandedLocations.Pop();
            NavAgent.SetDestination(nextLocation);
            return;
        }
        if(AutonomousLocations.Count > 0)
        {
            var nextLocation = AutonomousLocations.Pop();
            NavAgent.SetDestination(nextLocation);
        }
    }

    public void AddCommandedLocation(Vector3 location)
    {
        CommandedLocations.Push(location);
        ////If navmesh agent isn't doing anything at the moment then just pop it and start since user input is the highest priority
        //if (!NavAgent.hasPath)
        //{
        //    var nextLocation = CommandedLocations.Pop();
        //    NavAgent.SetDestination(nextLocation);
        //}
    }
    public void AddAutonomousLocation(Vector3 location)
    {
       // if (CommandedLocations.Count > 0)
       // {
            AutonomousLocations.Push(location);
       // }
       // else
       // {
       //     if (!NavAgent.hasPath)
       //     {
       //         var nextLocation = AutonomousLocations.Pop();
       //         NavAgent.SetDestination(nextLocation);
       //     }
       // }

    }

}


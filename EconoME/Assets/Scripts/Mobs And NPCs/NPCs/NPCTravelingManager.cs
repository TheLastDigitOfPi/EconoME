using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

/*

This will store all the possible waypoints any NPC can take.
 
 */
public class NPCTravelingManager : MonoBehaviour
{
    //Events
    public event Action OnStartTravelToDestination;
    public event Action OnTravelCancel;
    public event Action OnReachDestination;

    //Public fields
    public bool IsTraveling { get; private set; }

    //Local fields
    NPC _npc;
    [SerializeField] List<Transform> Waypoints;
    [SerializeField] NavMeshAgent pathfinder;
    [SerializeField] Animator _animator;

    WorldWayPointData _currentWayPoint;

    private void Awake()
    {
        _npc = GetComponent<NPC>();
    }

    private void Start()
    {
        _npc.OnStartTalkToPlayer += StartTalkToPlayer;
        _npc.OnEndTalkToPlayer += EndTalkToPlayer;
    }

    private void EndTalkToPlayer()
    {
        pathfinder.isStopped = false;
    }

    private void StartTalkToPlayer()
    {
        pathfinder.isStopped = true;
    }

    public bool GoToLocation(WorldLocationData destination)
    {
        _currentWayPoint = destination.GetWayPoint();
        if (!pathfinder.SetDestination(_currentWayPoint.WayPointWorldPosition))
            return false;

        OnStartTravelToDestination?.Invoke();
        IsTraveling = true;
        return true;
    }

    public bool GoToWayPoint(WorldWayPointData destination)
    {
        _currentWayPoint = destination;
        if (!pathfinder.SetDestination(_currentWayPoint.WayPointWorldPosition))
            return false;

        OnStartTravelToDestination?.Invoke();
        IsTraveling = true;
        return true;
    }

    public bool FindDesiredLocation()
    {
        if (!_npc.NPCScheduler.GetTravelLocation(out WorldLocationData location))
        {
            return false;
        }
        return true;
    }

    public void CancelTravel()
    {
        OnTravelCancel?.Invoke();
        pathfinder.isStopped = true;
        pathfinder.ResetPath();
    }

}

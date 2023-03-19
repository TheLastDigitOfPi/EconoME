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
    public event Action OnTravelResume;
    public event Action OnReachDestination;

    //Public fields
    public bool IsTraveling { get; private set; }

    //Local fields
    [SerializeField] string _idleAnimationName = "Idle";
    [SerializeField] string _moveAnimationName = "Walk";
    NPC _npc;
    [SerializeField] List<Transform> Waypoints;
    NavMeshAgent _pathfinder;
    Animator _animator;
    SpriteRenderer _renderer;
    bool _reachedDestination;

    Vector3 _currentDestination;



    WorldWayPointData _currentWayPoint;

    private void Awake()
    {
        _npc = GetComponent<NPC>();
        _animator = GetComponent<Animator>();
        _pathfinder = GetComponent<NavMeshAgent>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _npc.OnStartTalkToPlayer += InterruptTravel;
        _npc.OnEndTalkToPlayer += EndTalkToPlayer;
    }
    internal void StopTravel()
    {
        if (_reachedDestination)
            return;

        CancelTravel();
    }

    public bool TravelTickCheck()
    {
        _renderer.flipX = _currentDestination.x < _npc.transform.position.x;
        bool atDestination = _pathfinder.remainingDistance <= _pathfinder.stoppingDistance;
        if (atDestination)
        {
            FinishTravel();
        }
        return atDestination;
    }

    public void InterruptTravel()
    {
        _animator.CrossFade(_idleAnimationName, 0);
        OnTravelCancel?.Invoke();
        _pathfinder.velocity = Vector3.zero;
        _pathfinder.isStopped = true;
    }

    public void FinishTravel()
    {
        OnReachDestination?.Invoke();
        _pathfinder.isStopped = true;
        _reachedDestination = true;
        _currentDestination = Vector3.positiveInfinity;
        _animator.CrossFade(_idleAnimationName, 0);
    }

    public void ResumeTravel()
    {
        if (_currentDestination.Equals(Vector3.positiveInfinity))
            return;
        OnTravelResume?.Invoke();
        _pathfinder.isStopped = false;
        _animator.CrossFade(_moveAnimationName, 0);
        _pathfinder.SetDestination(_currentDestination);
    }

    private void EndTalkToPlayer()
    {
        _animator.CrossFade(_moveAnimationName, 0);
        _pathfinder.isStopped = false;
    }


    public bool GoToLocation(WorldLocationData destination)
    {
        if (!_pathfinder.SetDestination(destination.GetWayPoint().WayPointWorldPosition))
            return false;

        _reachedDestination = false;
        _pathfinder.isStopped = false;
        _currentWayPoint = destination.GetWayPoint();
        _currentDestination = _currentWayPoint.WayPointWorldPosition;
        _animator.CrossFade(_moveAnimationName, 0);
        OnStartTravelToDestination?.Invoke();
        IsTraveling = true;
        return true;
    }

    public bool GoToWayPoint(WorldWayPointData destination)
    {
        if (!_pathfinder.SetDestination(destination.WayPointWorldPosition))
            return false;

        _reachedDestination = false;
        _currentWayPoint = destination;
        _currentDestination = _currentWayPoint.WayPointWorldPosition;
        _animator.CrossFade(_moveAnimationName, 0);
        OnStartTravelToDestination?.Invoke();
        IsTraveling = true;
        return true;
    }

    public void CancelTravel()
    {
        OnTravelCancel?.Invoke();
        _pathfinder.isStopped = true;
        _pathfinder.velocity = Vector3.zero;
        _pathfinder.ResetPath();
        _animator.CrossFade(_idleAnimationName, 0);

    }

}

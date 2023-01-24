using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    StateMachine _stateMachine = new();
    [SerializeField] List<AIStateSO> _aIStates = new();
    [SerializeReference] List<AIState> _activeStates = new();
    public NavMeshAgent NavMeshAgent { get { return _navMeshAgent; } }
    NavMeshAgent _navMeshAgent;
    public bool isRunning = true;
    public event Action onDrawGizmos;
    private void OnDrawGizmos()
    {
        onDrawGizmos?.Invoke();
    }

    private void Start()
    {
        if (!TryGetComponent<NavMeshAgent>(out _navMeshAgent))
        {
            Debug.LogWarning("No Nav Mesh agent found", this); isRunning = false;
            return;
        }

        NavMeshAgent.updateRotation = false;
        NavMeshAgent.updateUpAxis = false;

        //Validate all AI States meet requirements
        foreach (var stateSO in _aIStates)
        {
            if (stateSO.GetAIState(this, out AIState state))
            {
                _activeStates.Add(state);
            }
        }

        //If no states remaining, whoops
        if (_activeStates.Count == 0)
        {
            Debug.LogWarning("AI Controller not able to load any states. Commensing Self Destruct!");
            Destroy(this);
            return;
        }

        //Add valid states to the state machine
        foreach (var state in _activeStates)
        {
            _stateMachine.AddAnyTransition(state, state.Condition);
        }
    }

    private void Update()
    {
        if (isRunning)
            _stateMachine.Tick();

    }


}

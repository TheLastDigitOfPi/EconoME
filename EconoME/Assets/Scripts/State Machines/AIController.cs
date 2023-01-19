using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    StateMachine _stateMachine = new();
    [SerializeField] List<AIState> _aIStates = new();
    [field: SerializeField] public NavMeshAgent NavMeshAgent {get; private set;}

    public bool isRunning = true;

    private void Start()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();

        if(NavMeshAgent== null){Debug.LogWarning("No Nav Mesh agent found", this); isRunning = false; return;}

        NavMeshAgent.updateRotation = false;
		NavMeshAgent.updateUpAxis = false;

        //Validate all AI States meet requirements
        foreach (var state in _aIStates)
        {
            state.OnStart(this);
        }

        //If no states remaining, whoops
        if (_aIStates.Count == 0)
        {
            Debug.LogWarning("AI Controller not able to load any states. Commensing Self Destruct!");
            Destroy(this);
            return;
        }

        //Add valid states to the state machine
        foreach (var state in _aIStates)
        {
            if (state.TransitionFrom != null)
            {
                _stateMachine.AddTransition(state.TransitionFrom, state, state.condition);
                continue;
            }
            _stateMachine.AddAnyTransition(state, state.condition);
        }
    }

    private void Update()
    {
        if (isRunning)
            _stateMachine.Tick();

    }

    public void FailedStateRequirements(AIState failedState, string message)
    {
        Debug.LogWarning("State failed to meet requirements: " + message);
        _aIStates.Remove(failedState);
    }
}

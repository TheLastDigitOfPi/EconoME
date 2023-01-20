using UnityEngine;
using System;

public abstract class AIStateSO : ScriptableObject
{
    public abstract bool GetAIState(AIController controller, out AIState state);
}

[System.Serializable]
public abstract class AIState : IState
{
    public bool PassedValidation { get; protected set; } = true;
    public Func<bool> Condition { get{return _condition.Condition;}}
    private AIStateCondition _condition;
    [SerializeField] protected AIStateConditionSO AICondition;
    protected AIController Controller;
    public AIState(){ }
    public AIState(AIController controller, AIStateConditionSO conditionSO)
    {
        Controller = controller;
        AICondition = conditionSO;
        _condition = AICondition.GetCondition();
        if (!_condition.CheckIfValid(controller, out string Error))
        {
            Debug.LogWarning("Condition '" + AICondition.GetType().Name + "' failed to meet requirements: " + Error, Controller.gameObject);
            PassedValidation = false;
        }
    }

    public void FailedStateRequirements(AIState failedState, string message)
    {
        Debug.LogWarning("State '" + failedState.GetType().Name + "' failed to meet requirements: " + message, Controller.gameObject);
        PassedValidation = false;
    }

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void Tick();
}

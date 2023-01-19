using UnityEngine;
using System;

public abstract class AIState : ScriptableObject, IState
{
    public Func<bool> condition { get { return AICondition.Condition; } }

    [field: SerializeField] public AIState TransitionFrom { get; private set; }

    [SerializeField] protected AIStateCondition AICondition;
    protected AIController Controller;

    protected Mob MobData;

    public void OnStart(AIController controller)
    {
        //Set Controller and Mob data
        Controller = controller;

        if (!Controller.TryGetComponent<Mob>(out MobData))
        {
            Controller.FailedStateRequirements(this, "No Mob Data Found");
        }


        //Check Func Conditions are met
        string conditionValidationMessage = AICondition.CheckIfValid(MobData);
        if (conditionValidationMessage != default)
        {
            Controller.FailedStateRequirements(this, conditionValidationMessage);
        }


        OnStart();
    }

    protected abstract void OnStart();

    public abstract void OnEnter();

    public abstract void OnExit();

    public abstract void Tick();
}

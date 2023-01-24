using UnityEngine;
using System;
//All Code by Daniel Davenport
#region HOW TO CREATE A NEW AI STATE
/*
The current system utilizes Scriptable Objects in order to quickly add AI to any AI controller

Explanation:
AI States (as well as conditions) must be checked for validation, meaning if the gameobject that is trying to utilize the AI does not have the required components (i.e. A collider or Some sort of Data storage like Mob data) then it will fail and not get added to the AI controller

Scriptable objects are forced to give the state they wish to create and add to the AI Controller. This simply means they must provide a new instnace of the AI state so that each controller that uses the state is seperate.

Steps:

Step 1 - Create a new class that extends IState. The class must have a defualt constructor so that we can create a default in our Unity UI for the scriptable object. 

Step 2 - Have the class validate that it can be used without issue. The class should have a constructor for validation when being added to an AI controller. The constructor can utilize the FailedStateRequirements() method when the state fails a validation check.

Step 3 - Create a Scriptable Object that extends the AIStateSO so that we can quickly apply the state to any AI controller (controller will just reject state if it fails). The scriptable object should have a AIState with a SerializedReference

See the AI State Template Region to easily set up a new AI State


*/
#endregion

#region AI STATE TEMPLATE
//Copy and past the State template and Ctrl F replace STATENAME with the state name you want
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class STATENAMESO : AIStateSO
{
    [SerializeReference] STATENAME STATENAMESettings = new();
    public override bool GetAIState(AIController controller, out AIState state)
    {
        state = new STATENAME(controller, TravelSettings);
        return state.PassedValidation;
    }
}

public class STATENAME : AIState
{
    
    public STATENAME(){ }
    public STATENAME(AIController controller, STATENAME other) : base(controller, other.AICondition)
    {

    }

    public override void OnEnter()
    {
        throw new NotImplementedException();
    }

    public override void OnExit()
    {
        throw new NotImplementedException();
    }

    public override void Tick()
    {
        throw new NotImplementedException();
    }
}


 
 
 
 
 
 
 */
#endregion

public abstract class AIStateSO : ScriptableObject
{
    /// <summary>
    /// Get the AI state for this type of AI. Will return a new AI state to add to the controller. If the state fails validation will return false
    /// </summary>
    /// <param name="controller">The AI controller we are attempted to add to</param>
    /// <param name="state">The state we are attempting to add to the AI Controller</param>
    /// <returns>Returns if the State passed validation</returns>
    public abstract bool GetAIState(AIController controller, out AIState state);
}

[System.Serializable]
public abstract class AIState : IState
{
    //Public Fields
    public bool PassedValidation { get; protected set; } = true;
    public Func<bool> Condition { get { return _condition.Condition; } }

    //Protected Fields
    [SerializeField] protected AIStateConditionSO AICondition;
    protected AIController Controller;
    //Local fields
    private AIStateCondition _condition;

    public AIState() { }
    public AIState(AIController controller, AIStateConditionSO conditionSO)
    {
        Controller = controller;
        AICondition = conditionSO;
        if(conditionSO == null)
        {
            PassedValidation = false;
            Debug.LogWarning("No condition found for the given state!");
            return;
        }
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

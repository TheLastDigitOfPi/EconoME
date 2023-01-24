using UnityEngine;
using System;


#region TEMPLATE

/*
 using UnityEngine;

[CreateAssetMenu(fileName = "New AISTATENAME", menuName = "ScriptableObjects/AI/Conditions/AISTATENAME")]
public class AISTATENAMESO : AIStateConditionSO
{
    public override AIStateCondition GetCondition()
    {
        return new AISTATENAME();
    }
}

public class AISTATENAME : AIStateCondition
{
    public override bool CheckIfValid(AIController controller, out string Error)
    {
        //Condition = () => mob.Health <= 0;
        Error = default;
        return true;
    }
}
*/


#endregion

[Serializable]
public abstract class AIStateConditionSO : ScriptableObject
{
    public abstract AIStateCondition GetCondition();
}

public abstract class AIStateCondition
{
    public Func<bool> Condition { get; protected set; }
    public abstract bool CheckIfValid(AIController controller, out string Error);
}

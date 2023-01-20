using UnityEngine;
using System;

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

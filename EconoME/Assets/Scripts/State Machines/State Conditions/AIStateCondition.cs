using UnityEngine;
using System;

[Serializable]
public abstract class AIStateCondition : ScriptableObject
{
    public Func<bool> Condition { get; protected set; }

    public abstract string CheckIfValid(Mob mobData);
}

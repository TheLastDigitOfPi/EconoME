using UnityEngine;

[CreateAssetMenu(fileName = "New Is Wandering", menuName = "ScriptableObjects/AI/Conditions/IsWandering")]
public class IsWandering : AIStateCondition
{
    public override string CheckIfValid(Mob mobData)
    {
        Condition = () => mobData.IsAlive && mobData.IsNeutral;
        
        return default;
    }

}

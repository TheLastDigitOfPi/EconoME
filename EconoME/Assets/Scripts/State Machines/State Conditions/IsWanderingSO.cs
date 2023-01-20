using UnityEngine;

[CreateAssetMenu(fileName = "New Is Wandering", menuName = "ScriptableObjects/AI/Conditions/IsWandering")]
public class IsWanderingSO : AIStateConditionSO
{
    public override AIStateCondition GetCondition()
    {
        return new IsWandering();
    }
}
public class IsWandering : AIStateCondition
{
    public override bool CheckIfValid(AIController controller, out string Error)
    {
        if (!controller.TryGetComponent(out Mob mobData))
        {
            Error = "Failed to find mob class";
            return false;

        }
        Condition = () => mobData.IsAlive && mobData.IsNeutral;
        Error = default;
        return true;
    }

}

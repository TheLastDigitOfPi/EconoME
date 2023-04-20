using UnityEngine;

[CreateAssetMenu(fileName = "New Is Dead Condition", menuName = "ScriptableObjects/AI/Conditions/IsDead")]
public class IsDeadSO : AIStateConditionSO
{
    public override AIStateCondition GetCondition()
    {
        return new IsDead();
    }
}

public class IsDead : AIStateCondition
{
    public override bool CheckIfValid(AIController controller, out string Error)
    {
        if (!controller.TryGetComponent(out Mob mob))
        {
            Error = "Failed to find mob class";
            return false;
        }

        Condition = () => mob.Health.CurrentHealth <= 0;
        Error = default;
        return true;
    }
}



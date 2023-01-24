using UnityEngine;

[CreateAssetMenu(fileName = "New Is Attacking", menuName = "ScriptableObjects/AI/Conditions/IsAttacking")]
public class IsAttackingSO : AIStateConditionSO
{
    public override AIStateCondition GetCondition()
    {
        return new IsAttacking();
    }
}

public class IsAttacking : AIStateCondition
{
    public override bool CheckIfValid(AIController controller, out string Error)
    {
        if(!controller.TryGetComponent(out Mob mobData))
        {
            Error = "Unable to find Mob Class";
            return false;
        }

        Condition = () => mobData.IsAlive && mobData.IsAngry;
        Error = default;
        return true;
    }
}
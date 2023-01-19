using UnityEngine;

[CreateAssetMenu(fileName = "New Is Attacking", menuName = "ScriptableObjects/AI/Conditions/IsAttacking")]
public class IsAttacking : AIStateCondition
{
    public override string CheckIfValid(Mob mobData)
    {
        Condition = () => mobData.IsAlive && mobData.IsAngry;
        
        return default;
    }

}
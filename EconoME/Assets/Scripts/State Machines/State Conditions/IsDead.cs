using UnityEngine;

[CreateAssetMenu(fileName = "New Is Dead Condition", menuName = "ScriptableObjects/AI/Conditions/IsDead")]
public class IsDead : AIStateCondition
{
    public override string CheckIfValid(Mob mobData)
    {
        Condition = () => mobData.Health <= 0;
        
        return default;
    }

}



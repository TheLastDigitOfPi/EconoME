using UnityEngine;

public class TestingCombatController : EntityCombatController
{
    public override CombatDamageReport ReceiveAttack(CombatAttackInstance attack)
    {
        Debug.Log("I was hit");
        return null;
    }
}

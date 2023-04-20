using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAttackRangeChecker : MonoBehaviour
{
    [SerializeReference] IMobWithAttack ParentMob;
    [SerializeField] Mob parentMob;
    private void Awake()
    {
        ParentMob = GetComponentInParent<IMobWithAttack>();
        parentMob.Health.OnNoHealth += () => Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EntityCombatController controller))
            ParentMob.TargetEnterAttackRange();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EntityCombatController controller))
            ParentMob.TargetExitAttackRange();

    }
}

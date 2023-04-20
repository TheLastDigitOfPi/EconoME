using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAggroRangeChecker : MonoBehaviour
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
            ParentMob.TargetEnterAggroRange(controller);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EntityCombatController controller))
            ParentMob.TargetExitAggroRange(controller);

    }
}

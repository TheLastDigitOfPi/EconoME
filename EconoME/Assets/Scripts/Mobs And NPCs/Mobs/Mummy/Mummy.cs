using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Mummy : Mob, IKnockbackable, IMobWithAttack
{
    [SerializeField] string _attackName = "Whip Attack";
    public EntityCombatController Target { get; private set; }

    public event Action OnTargetEnterAggroRange;
    public event Action OnTargetLeaveAggroRange;
    public event Action OnTargetEnterAttackRange;
    public event Action OnTargetLeaveAttackRange;

    [SerializeField] float _attackCooldown = 1;
    [SerializeField] bool _canAttack = true;
    [SerializeField] AttackObject attackObject;
    [SerializeReference] Weapon weapon = new();
    public void Knockback(float distance, Vector2 direction)
    {
        CombatExtensions.DoKnockback(this, Agent, Rigidbody, distance, direction);
    }

    public override bool MoveTowardsPosition(Vector3 position)
    {
        if (!_canAttack)
            return false;
        return base.MoveTowardsPosition(position);
    }

    public bool TryAttack(EntityCombatController target)
    {
        if (!_canAttack || Health.CurrentHealth <= 0)
            return false;


        Animator.CrossFade(_attackName, 0);
        if (Agent.isOnNavMesh)
            Agent.isStopped = true;
        StartCoroutine(AttackCooldown());
        IEnumerator AttackCooldown()
        {
            do
            {
                _canAttack = false;
                CanIdle = false;
                yield return new WaitForSeconds(0.3f);
                if (Health.CurrentHealth <= 0)
                    break;
                FacingDirection = target.transform.position.x > transform.position.x ? MoveDirection.Right : MoveDirection.Left;
                Renderer.flipX = FacingDirection == MoveDirection.Left;
                var offset = (target.transform.position - transform.position).normalized * 0.7f;

                yield return new WaitForSeconds(0.3f);

                var attack = Instantiate(attackObject);
                attack.Initialize(weapon, this, offset);
                attack.transform.localScale = Vector3.one * 0.5f;

                if (Health.CurrentHealth <= 0)
                    Agent.isStopped = false;
                yield return new WaitForSeconds(_attackCooldown);
                _canAttack = true;
                CanIdle = true;
            } while (false);

        }
        return true;
    }

    public bool TryGetTarget(out EntityCombatController target)
    {
        target = Target;
        return Target != null;
    }

    public override bool TryReceiveAttack(Action<EntityCombatController> onAttack, out CombatDamageReport report)
    {
        onAttack?.Invoke(this);
        report = CombatDamageReport.ImmunityReport;
        return true;
    }

    public void TargetEnterAggroRange(EntityCombatController target)
    {
        if (Target == null)
            Target = target;
        OnTargetEnterAggroRange?.Invoke();
    }

    public void TargetExitAggroRange(EntityCombatController target)
    {
        if (Target != target)
            return;
        Target = null;
        OnTargetLeaveAggroRange?.Invoke();
    }

    public void TargetEnterAttackRange()
    {
        OnTargetEnterAttackRange?.Invoke();
    }

    public void TargetExitAttackRange()
    {
        OnTargetLeaveAttackRange?.Invoke();
    }
}
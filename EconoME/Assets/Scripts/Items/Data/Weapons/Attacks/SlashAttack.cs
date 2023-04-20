using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SlashAttack : AttackObject
{
    Weapon _weapon;
    EntityCombatController _attacker;
    [SerializeField] SpriteRenderer _spriteRenderer;
    bool attackTimerFinished = false;
    public Vector2 Offset;
    public override void Initialize(Weapon attackWeapon, EntityCombatController attacker, Vector2 direction)
    {
        _weapon = attackWeapon;
        _attacker = attacker;
        Offset = direction;
    }

    private void Start()
    {
        _spriteRenderer.flipX = Offset.x >= 0;
        transform.position = _attacker.transform.position + Offset.ToVector3();
        StartCoroutine(StartAttackTimer());
        StartCoroutine(DestroyAfterComplete());
        IEnumerator DestroyAfterComplete()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(this.gameObject);
        }
    }

    IEnumerator StartAttackTimer()
    {
        yield return new WaitForSeconds(0.2f);
        attackTimerFinished = true;
    }

    List<EntityCombatController> HitEnemies = new();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(attackTimerFinished)
            return;
        if (collision.TryGetComponent(out EntityCombatController entity))
        {
            if (entity == _attacker || HitEnemies.Contains(entity))
                return;
            HitEnemies.Add(entity);
            CombatAttackInstance attackInstance = new(_attacker, _weapon.CurrentDamage);
            Debug.Log("Hit Entity: " + entity.name);

            if (!entity.TryReceiveAttack(_weapon.DoAttack, out var report))
                return;
            if (entity is IKnockbackable)
            {
                var knockbacktarget = entity as IKnockbackable;
                var direction = -1 * (_attacker.transform.position - entity.transform.position).normalized;
                knockbacktarget.Knockback(5, direction);
            }
            _weapon.AttackLanded(report);
        }

    }
}
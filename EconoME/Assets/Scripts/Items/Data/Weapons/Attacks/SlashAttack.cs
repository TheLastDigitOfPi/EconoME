using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAttack : AttackObject
{
    Weapon _weapon;
    EntityCombatController _attacker;
    [SerializeField] SpriteRenderer _spriteRenderer;
    public override void Initialize(Weapon attackWeapon, EntityCombatController attacker)
    {
        _weapon = attackWeapon;
        _attacker = attacker;
    }

    private void Start()
    {
        var offset = Vector3.right * 1 + Vector3.up * 0.2f;
        _spriteRenderer.flipX = true;
        switch (_attacker.FacingDirection)
        {
            case MoveDirection.Left:
                offset = Vector3.left * 1 + Vector3.up * 0.2f;
                _spriteRenderer.flipX = false;
                break;
            default:
                break;
        }
        transform.position = _attacker.transform.position + offset;
        StartCoroutine(DestroyAfterComplete());
        IEnumerator DestroyAfterComplete()
        {
            yield return new WaitForSeconds(0.5f);
            Destroy(this.gameObject);
        }
    }

    List<EntityCombatController> HitEnemies = new();
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EntityCombatController entity))
        {
            if (entity == _attacker || HitEnemies.Contains(entity))
                return;
            HitEnemies.Add(entity);
            CombatAttackInstance attackInstance = new(_attacker, _weapon.CurrentDamage);
            Debug.Log("Hit Entity: " + entity.name);
            var report = entity.ReceiveAttack(attackInstance);
            _weapon.AttackLanded(report);
        }

    }
}
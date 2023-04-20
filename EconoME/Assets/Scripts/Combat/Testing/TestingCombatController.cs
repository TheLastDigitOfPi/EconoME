using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class TestingCombatController : EntityCombatController, IKnockbackable
{
    Rigidbody2D _rigidbody;
    NavMeshAgent _agent;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Death()
    {
        onDeath?.Invoke();
        Destroy(this.gameObject);
    }

    [SerializeField] int hitsTillDealth = 5000;

    
    public override bool TryReceiveAttack(Action<EntityCombatController> onAttack, out CombatDamageReport report)
    {
        //Get direction attacker is in
        report = null;

        Debug.Log("I was hit");
        hitsTillDealth--;
        if (hitsTillDealth <= 0)
            Death();
        return false;
    }

    public void Knockback(float distance, Vector2 direction)
    {
        CombatExtensions.DoKnockback(this, _agent, _rigidbody, distance, direction);
    }
}

public interface IKnockbackable
{
    public void Knockback(float distance, Vector2 direction);
}
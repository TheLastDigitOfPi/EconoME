using System;
using UnityEngine;

public class SquirrelMob : Mob, IKnockbackable
{
    [SerializeField] public override HealthStat Health {get; protected set;}

    [ContextMenu("Kill IT!")]
    public void KillMob()
    {
        Health.Heal(-1 * Health.CurrentHealth);
    }

    public void Knockback(float distance, Vector2 direction)
    {
    }

    public override void OnMobDeath()
    {
        Debug.Log("Squirrel is dead");
    }

    public override bool TryReceiveAttack(Action<EntityCombatController> onAttack, out CombatDamageReport report)
    {
        //Get direction attacker is in
        report = null;
        Debug.Log("I was hit");
        return false;
    }
}


public interface IDamageable
{
    public HealthStat Health { get; }
}


public interface ICombatable : IDamageable
{
    public HealthStat Health2 { get; }
}

public interface ICanCastSpells
{
    public event Action OnCastSpell;
}
using UnityEngine;

public class TestCombatEntity : EntityCombatController
{
    /*
    I like the idea of each combat entity having a set of attacks that can choose from.
    
    A combat entity will likely either do a direct attack or create some form of lasting effect that will do the attack (like a projectile)

    Direction attacking would be like running into the entity, possibly not something we want.
    
     */


    
    #region CreatingAProjectile

    void ProjectileAttack()
    {

        GameObject projectilePrefab = new();
        var projectile = Instantiate(projectilePrefab);
        //Initialize the projectile with the attack
        //projectile.GetComponent<ProjectileHandler>().Initialize(attack);

    }

    #endregion

    #region DirectlyAttacking
    void DirectAttack()
    {
        GameObject projectilePrefab = new();
        var projectile = Instantiate(projectilePrefab);
        //Initialize the projectile with the attack
        //projectile.GetComponent<ProjectileHandler>().Initialize(attack);
    }

    WeaponDamage _baseAttack;


    void BasicHit(EntityCombatController target)
    {
        var entityTransform = target.transform;

        //Call lightning strike on target at target position
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out EntityCombatController foundentity))
        {
            CombatAttackInstance attack = new(this, _baseAttack);
            foundentity.ReceiveAttack(attack);
        }
    }

    public override CombatDamageReport ReceiveAttack(CombatAttackInstance attack)
    {
         attack.OnAttackHit.Invoke(this);

        return CombatDamageReport.ImmunityReport;
    }
    #endregion
}

#region Thoughts
/*

The damage instance needs 3 main things

1) A reference to the entity that made this attack

2) The damage values used to calculate how much damage is being done to the target
 
3) Any special effects that the attack contains

Special Effect Examples:

On landing hit, apply poison to the entity
On hit, lower the entities defense
On hit, steal HP


More complicated Special Effect possibilities
On land, create fire around the entity
On land, stike lightning on the entity

In order to calculate our damage, we should pass some kind of damage class instance with the damage we are trying to deal

For example, a damage instance could be: I am going to deal 10 normal damage to the target when hit

This leaves open room for different types of calculations that can be done, some of which could include:
Piercing Damage - damage that ignores the targets defense stat
Normal Damage - default damage type
Ranged Damage - Damage that is only defended against with range defense stats
Magic Damage - Damage that is only defended against with magic resist

If we want to make it crazy complicated / "extensive", simply add another type of damage type that follows some sort of rule
Slashing Damage - 
Summoning Damge -
Explosive Damage -

If we want to get sued by GameFreak
Psycic damage -
Dark damage - 
Any Elemental Damage (water, ice, fire, earth, air, etc) - Could be a subset of magic damage, or could be a subset of all damage types

 */
#endregion
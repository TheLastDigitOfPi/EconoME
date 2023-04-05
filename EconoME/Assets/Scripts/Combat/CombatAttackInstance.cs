using System;

/// <summary>
/// Represents a damaging instance being done to an entity that stores data such as the attack numbers, who is attacking, etc
/// </summary>
[Serializable]
public class CombatAttackInstance
{
    /// <summary>
    /// Subscribe to this event with what should happen when the entity is hit with the attack
    /// </summary>
    /// <remarks>
    /// This event needs to be called by someone, but who?
    /// Option A - The event is called in this class, keeping it nice and organized
    /// Option B - The event is called in the person getting hit. This makes it easy to send over who is being attacked to the attacker
    /// </remarks>
    public Action<EntityCombatController> OnAttackHit;
    public Action<EntityCombatController> OnAttackFail;
    public CombatDamageInstance Attack { get; private set; }
    public EntityCombatController Attacker { get; private set; }

    public CombatAttackInstance(EntityCombatController attacker, CombatDamageInstance attack)
    {
        Attack = attack;
        Attacker = attacker;
    }

    public CombatDamageInstance CalculateAttack(EntityCombatController defenses)
    {
        return new CombatDamageInstance();
    }

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
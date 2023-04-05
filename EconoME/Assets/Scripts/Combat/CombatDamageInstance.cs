using System;

/// <summary>
/// The damage values used to calculate the damage being done
/// </summary>
[Serializable]
public struct CombatDamageInstance
{
    public float NeturalDamage;
    public float RangedDamage;
    public float MagicDamage;
    public float PhysicalDamage;
    public float TrueDamage;
    public float ElementalDamage;
}

public struct CombatDefenseInstance
{
    public float NeturalDamage;
    public float RangedDamage;
    public float MagicDamage;
    public float PhysicalDamage;
    public float TrueDamage;
    public float ElementalDamage;
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
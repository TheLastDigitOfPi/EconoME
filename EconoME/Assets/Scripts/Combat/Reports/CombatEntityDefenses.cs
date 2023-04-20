using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct StatChanger : IEquatable<StatChanger>
{
    public float BasePercentChange;
    public float BaseAdditionChange;
    public float PerecentChange;
    public float AddtionChange;

    public bool Equals(StatChanger other)
    {
        return other.BasePercentChange == this.BasePercentChange
            && other.BaseAdditionChange == this.BaseAdditionChange
            && other.PerecentChange == this.PerecentChange
            && other.AddtionChange == this.AddtionChange;
    }

    public override bool Equals(object obj)
    {
        return false;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(StatChanger c1, StatChanger c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(StatChanger c1, StatChanger c2)
    {
        return !c1.Equals(c2);
    }

    public static StatChanger operator +(StatChanger first, StatChanger second)
    {
        StatChanger result = new()
        {
            BasePercentChange = first.BasePercentChange + second.BasePercentChange,
            BaseAdditionChange = first.BaseAdditionChange + second.BaseAdditionChange,
            PerecentChange = first.PerecentChange + second.PerecentChange,
            AddtionChange = first.AddtionChange + second.AddtionChange
        };
        return result;
    }
}

[System.Serializable]
public struct CombatEntityDefenses
{
    public float CurrentHealth;
    public float MaxHealth;
    public float HealthPerSecond;
    public float PhysicalDefense;
    public float MagicalDefense;
    public float RangedDefense;
    public float TrueDefense;
    public float CurrentHealthPercent { get { return CurrentHealth / MaxHealth; } }
    public void CalculateBaseStatChange(DefenseStatChangeInstance statChange)
    {
        CalculateBaseChange(statChange.HealthChanges, ref MaxHealth);
        CalculateBaseChange(statChange.PhysicalDefenseChanges, ref PhysicalDefense);
        CalculateBaseChange(statChange.MagicalDefenseChanges, ref MagicalDefense);
        CalculateBaseChange(statChange.RangedDefenseChanges, ref RangedDefense);
        CalculateBaseChange(statChange.TrueDefenseChanges, ref TrueDefense);
        CalculateBaseChange(statChange.HPSChanges, ref HealthPerSecond);

        void CalculateBaseChange(StatChanger stat, ref float changingStat)
        {
            changingStat += (stat.BasePercentChange / 100f) * changingStat;
            changingStat += stat.BaseAdditionChange;
        }
    }

    public void CalculateSecondStatChange(DefenseStatChangeInstance statChange)
    {
        CalculateSecondChange(statChange.HealthChanges, ref MaxHealth);
        CalculateSecondChange(statChange.PhysicalDefenseChanges, ref PhysicalDefense);
        CalculateSecondChange(statChange.MagicalDefenseChanges, ref MagicalDefense);
        CalculateSecondChange(statChange.RangedDefenseChanges, ref RangedDefense);
        CalculateSecondChange(statChange.TrueDefenseChanges, ref TrueDefense);

        void CalculateSecondChange(StatChanger stat, ref float changingStat)
        {
            changingStat += (stat.PerecentChange / 100f) * changingStat;
            changingStat += stat.AddtionChange;
        }
    }

    public CombatEntityDefenses CalcuateAllChanges(List<DefenseStatChangeInstance> changes, float health)
    {
        foreach (var modifier in changes)
        {
            CalculateBaseStatChange(modifier);
        }
        foreach (var modifier in changes)
        {
            CalculateSecondStatChange(modifier);
        }
        CurrentHealth = health > MaxHealth ? MaxHealth : health;
        return this;
    }

    internal CombatEntityDefenses Heal(float healAmount)
    {
        if (healAmount <= 0)
            return this;
        CurrentHealth += healAmount;
        if (CurrentHealth > MaxHealth)
            CurrentHealth = MaxHealth;
        return this;
    }

    #region Thoughts
    /*
     
    We need a way to calculate damage done to the player, as well as special effects that could happen when the player is hit.

    Each step should have some kind of defined level. That way we can create effects that are done before or after certain calculations

    First, we should have some way of defining how the base damage is calculated
    Ex: Say we have 10 defense and are taking 20 damage, simple solution is we are taking (20-10) 10 total damage.

    Then we need to apply special effects done by the armor.
    Ex: The armor provides 25% damage mitigation, so we go from 10 -> 8 damage being done.
     
    What if the attacker has some kind of special effect? Maybe they ignore armor or do more if you are wearing a certain type of armor
    Ignoring Armor -> Just send in damage as True Damage
    Damage on certain type of armor -> Should be calculated by armor wearer (i.e. leather armor takes more fire damage)

    How do we add effects to attacks like poison, slowness, etc.


    To caluclate these we need a few sections.

    Section 1: Pre Calculation Base value changes
    Make changes to the base value
    The base value is never technically changed, but these changes are directly applied to the base value

    Section 2: Normal calculation value changes
    Add up changes not to the base value

    Section 3: Post calculation value changes
    These changes are applied after, commonly addtion or subtraction that we don't want on the base value. Very rare, possibly never used


    Simple Ex:

    Base Defense value of 20
    Armor gives +5 bonus defense for each something, totaling +10

    20 + 10 = 30

    We are currently debuffed from a monster so our armor is reduced by 20%

    30 * 0.8 = 24
     
    Defense is now 24

    Medium Ex:
    Our armor has a total of 40 base defense. One piece gives -15% defense when holding a stick idk. Another piece gives +20% defense when it is night time idk. Lets say the conditions are being met for the bonuses.

    Part 1: Base Calculation
    Base = 40

    Part 2: Post-Base Calculation
    40 * (-15 + 20 = 5%, to decimal -> = 1.05) = 42



    Complicated Ex:

    Currently Wearing 4 pieces of armor
    Helmet - 10 defense. Blocks 1 hit of magical damage every 10 seconds.
    Chestplate - 20 defense. Bonus 5% base armor for each piece of metal armor on.(20% total)
    Leggings - 15 defense. 20% bonus armor at night. 
    Boots - 5 defense 15% bonus armor when standing still




     */
    #endregion


}
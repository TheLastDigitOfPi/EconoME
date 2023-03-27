using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : EntityCombatController
{
    //Statics
    public static PlayerCombatController Instance { get; private set; }

    //Public fields
    public float PlayerCurrentHealth { get { return _playerStats.CurrentHealth; } }

    //Events
    public event Action OnPlayerHit;
    public event Action OnPlayerDeath;
    public event Action OnPlayerFullyHealed;
    public event Action OnPlayerPassiveHeal;
    public event Action OnPlayerAttack;
    public event Action OnPlayerCastSpell;

    public event Action<CombatDamageReport> OnPlayerCombatReportDefense;
    public event Action<CombatDamageReport> OnPlayerCombatReportAttack;

    [SerializeField] CombatEntityDefenses _playerStats = new();
    [SerializeField] InventoryObject _armorSlots;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 player combat controller found!");
            Destroy(this);
            return;
        }
        Instance = this;

        foreach (var slot in _armorSlots.Data.ItemSlots)
        {
            slot.OnItemChange += OnArmorChange;
        }
    }

    private void OnArmorChange(Item item)
    {
        if (item is not Armor)
            return;

        var armorItem = item as Armor;
        OnPlayerDefenseCalculation += (stats) =>
        {


        };

    }

    public event Action<CombatEntityDefenses> OnPlayerDefenseCalculation;

    public override CombatDamageReport ReceiveAttack(CombatAttackInstance attack)
    {
        var defenses = CalculateDefenses();

        OnPlayerDefenseCalculation?.Invoke(_playerStats);




        /*
        Here is the current dilemma.
        Calculating this is fine, but what happens with these special cases where maybe we have the ability to block a single instance of damage on a cooldown
        How do we create some form of decoupled code that allows us to just stop executing if we need to.
        What if there is a scenario where we should do something after the block, where the order matters?
        Such as maybe an armor piece that gives % mitagation after blocking a hit idk.

        On one hand, we can just hard code in blocking and have armor specifically subscribe to when we block do something.
         */
        var combatReport = new CombatDamageReport();

        //Let anyone that wants to know about a defensive report know what happened
        OnPlayerCombatReportDefense?.Invoke(combatReport);
        return combatReport;
    }

    CombatEntityDefenses CalculateDefenses()
    {
        return new CombatEntityDefenses();
    }
}

public struct CombatDamageReport
{
    public float DamageDone;
    public float DamageMitagated;
    public bool KilledEntity;
    public bool BlockedAttack { get { return DamageDone <= 0; } }

    public CombatDamageReport(float damageDone, float damageMitagated, bool killedEntity)
    {
        DamageDone = damageDone;
        DamageMitagated = damageMitagated;
        KilledEntity = killedEntity;
    }
}

public abstract class EntityCombatController : MonoBehaviour, ICombatEntity
{
    [SerializeField] public CombatEntityDefenses EntityStats { get; protected set; }
    public abstract CombatDamageReport ReceiveAttack(CombatAttackInstance attack);
}

public class CombatEntityDefenses
{
    public float CurrentHealth;
    public float MaxHealth;
    public float PhysicalDefense;
    public float MagicalDefense;
    public float RangedDefense;
    public float BasicDefense;

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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerCombatController : EntityCombatController
{
    //Statics
    public static PlayerCombatController Instance { get; private set; }

    //Public fields
    public float PlayerCurrentHealth { get { return EntityStartingStats.CurrentHealth; } }
    [field: SerializeField] public bool ImmunityFramesActive { get; private set; } = false;

    public event Action<CombatDamageReport> OnPlayerCompleteDefenseReport;
    public event Action<CombatDamageReport> OnPlayerCombatReportAttack;

    //Local fields
    [SerializeField] InventoryObject _armorSlots;
    [SerializeReference] Weapon _currentHeldWeapon;
    [field: SerializeField] public ModifiableStat AttackSpeed { get; private set; }

    //Helpers
    bool HoldingWeapon { get { return _currentHeldWeapon != null; } }
    public bool CanMove
    {
        get
        {
            if (!HoldingWeapon)
                return true;
            return !_currentHeldWeapon.Attacking;
        }
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 player combat controller found!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        foreach (var slot in _armorSlots.Data.ItemSlots)
        {
            slot.OnItemAdded += ArmorEquiped;
            slot.OnItemRemoved += ArmorRemoved;
            if (slot.HasItem)
            {
                var armor = slot.Item as Armor;
                armor.Equip(this);
            }
        }

        UpdateDefenses(useBaseStats: true);
        OnPlayerCompleteDefenseReport += CheckReport;
        WorldTimeManager.OnGameTick += PassiveRegen;
        HotBarHandler.Instance.OnSelectItem += HotbarSelectedItem;
        HotBarHandler.Instance.OnDeselectItem += HotbarDeselectItem;
        RaycastHandler.Instance.OnRaycastFail += AttemptAttack;
        PlayerMovementController.Instance.OnPlayerStartMove += UpdateDirection;
    }

    private void UpdateDirection()
    {
        FacingDirection = PlayerMovementController.Instance.CurrentDirection;
    }

    private void AttemptAttack()
    {
        if (!HoldingWeapon)
            return;
        _currentHeldWeapon.Attack(this);
    }

    private void HotbarDeselectItem(Item obj)
    {
        if (obj != _currentHeldWeapon || !HoldingWeapon)
            return;
        _currentHeldWeapon?.Unequip();
        _currentHeldWeapon = default;
    }

    private void HotbarSelectedItem(Item item)
    {
        if (item is Weapon)
        {
            _currentHeldWeapon = item as Weapon;
            _currentHeldWeapon.Equip(this);
        }
    }

    private void OnDestroy()
    {
        foreach (var slot in _armorSlots.Data.ItemSlots)
        {
            if (slot.HasItem)
            {
                var armor = slot.Item as Armor;
                armor.Unequip();
            }
        }
    }

    private void CheckReport(CombatDamageReport report)
    {
        if (report.KilledEntity)
            PlayerDeath();
    }

    void PlayerDeath()
    {
        Debug.Log("Player Dead");
    }

    private void ArmorRemoved(Item item)
    {
        if (item is not Armor)
            return;

        var armor = item as Armor;
        armor.Unequip();
    }
    private void ArmorEquiped(Item item)
    {
        if (item is not Armor)
            return;

        var armor = item as Armor;
        armor.Equip(this);
    }

    [SerializeField] float HealPerSecond = 0.25f;
    void PassiveRegen()
    {
        if (WorldTimeManager.CurrentTime.CurrentTick % WorldTimeManager.CurrentTime.TicksPerSecond != 0)
            return;
        EntityCurrentStats = EntityCurrentStats.Heal(EntityCurrentStats.HealthPerSecond);
    }

    public override CombatDamageReport ReceiveAttack(CombatAttackInstance attack)
    {
        if (ImmunityFramesActive)
            return CombatDamageReport.ImmunityReport;


        CombatDamageReport report = new(attack, this);
        report.CalculateInitalDamage();
        onDamageReportCalculation.Invoke(report);
        report.RecalculateDamage();


        /*
        Here is the current dilemma.
        Calculating this is fine, but what happens with these special cases where maybe we have the ability to block a single instance of damage on a cooldown
        How do we create some form of decoupled code that allows us to just stop executing if we need to.
        What if there is a scenario where we should do something after the block, where the order matters?
        Such as maybe an armor piece that gives % mitagation after blocking a hit idk.


        Chance to block attack
        Every x hits reduce damage taken by 5


        On one hand, we can just hard code in blocking and have armor specifically subscribe to when we block do something.
         
        So for now this is how it is going to work
        Enchantments can subscribe to events and do certain actions based on that event
        Any enchantment that needs to manipulate some form of data on an action (Ex: Add defense to player when hit 5 times idk) will subscribe to an action that gives data that can be manipulated

         
         */


        //Let anyone that wants to know about a defensive report know what happened
        OnPlayerCompleteDefenseReport?.Invoke(report);
        onDefense?.Invoke();
        StartCoroutine(StartImmunityFrames());

        return report;
    }

    IEnumerator StartImmunityFrames()
    {
        ImmunityFramesActive = true;
        yield return null;
        ImmunityFramesActive = false;
    }

}

public class CombatDamageReport
{
    public EntityCombatController Defender { get; private set; }
    public CombatAttackInstance Attack { get; private set; }
    public float DamageDone { get; private set; }
    public float DamageMitagated { get; private set; }
    public bool KilledEntity { get; private set; }
    public bool BlockedAttack { get { return DamageDone <= 0; } }

    public bool Immune { get; private set; }

    public bool StopCalculating { get { return BlockedAttack || KilledEntity; } }
    public CombatDamageReport(CombatAttackInstance attack, EntityCombatController defender)
    {
        Attack = attack;
        Defender = defender;
    }

    CombatDamageReport(bool immune)
    {
        Immune = immune;
    }

    List<CombatReportModifier> modifiers = new();
    public void AddReportModifier(CombatReportModifier modifier)
    {
        modifiers.Add(modifier);
    }

    public CombatDamageInstance CurrentDamage;
    public CombatEntityDefenses CurrentDefenses;
    public DefenseStatChangeInstance DefenseChanges;
    public void RecalculateDamage()
    {
        var flatChanges = modifiers.FindAll(m => m.Priority == CombatReportPriority.FlatStatChanges);
        foreach (var change in flatChanges)
        {
            change.ApplyModifier(this);
        }
        CurrentDefenses.CalculateBaseStatChange(DefenseChanges);
        CurrentDefenses.CalculateSecondStatChange(DefenseChanges);

        var otherModifiers = modifiers.FindAll(m => m.Priority != CombatReportPriority.FlatStatChanges).ToList();
        foreach (var item in modifiers)
        {
            item.ApplyModifier(this);
        }

    }



    public static CombatDamageReport ImmunityReport { get { return new CombatDamageReport(true); } }

    public void CalculateInitalDamage()
    {
        var attack = Attack.Attack;
    }
}

public enum CombatReportPriority
{
    FlatStatChanges,
    FancyStatChanges,
}

[System.Serializable]
public abstract class CombatReportModifier
{
    public CombatReportPriority Priority;
    public abstract void ApplyModifier(CombatDamageReport report);
}

public abstract class EntityCombatController : MonoBehaviour
{
    [field: SerializeField] public CombatEntityDefenses EntityStartingStats { get; protected set; }
    [field: SerializeField] public CombatEntityDefenses EntityCurrentStats { get; protected set; }
    [field: SerializeField] public CombatEntityType EntityType { get; protected set; }
    [SerializeField] protected List<DefenseStatChangeInstance> _statModifiers = new();
    public MoveDirection FacingDirection { get; protected set; }
    public event Action OnDeath;
    protected Action onDeath { get { return OnDeath; } set { OnDeath = value; } }

    public event Action OnFullyHealed;
    protected Action onFullyHealed { get { return onFullyHealed; } set { onFullyHealed = value; } }

    public event Action OnPassiveHeal;
    protected Action onPassiveHeal { get { return OnPassiveHeal; } set { OnPassiveHeal = value; } }

    public event Action OnAttack;
    protected Action onAttack { get { return OnAttack; } set { OnAttack = value; } }

    public event Action OnDefense;
    protected Action onDefense { get { return OnDefense; } set { OnDefense = value; } }

    public event Action OnCastSpell;
    protected Action onCastSpell { get { return OnCastSpell; } set { OnCastSpell = value; } }


    public event Action<CombatDamageReport> OnDamageReportCalculation;
    protected Action<CombatDamageReport> onDamageReportCalculation { get { return OnDamageReportCalculation; } set { OnDamageReportCalculation = value; } }

    /// <summary>
    /// Called when an entity has landed an attack on this entity
    /// </summary>
    /// <param name="attack"></param>
    /// <returns></returns>
    public abstract CombatDamageReport ReceiveAttack(CombatAttackInstance attack);
    public void TestHit()
    {
        onDefense?.Invoke();
    }

    public void AddStatChange(DefenseStatChangeInstance modifier)
    {
        _statModifiers.Add(modifier);
        UpdateDefenses();
    }

    public void RemoveStatChange(DefenseStatChangeInstance modifier)
    {
        _statModifiers.Remove(modifier);
        UpdateDefenses();
    }
    protected void UpdateDefenses(bool useBaseStats = false)
    {
        var originalHealth = useBaseStats ? EntityStartingStats.CurrentHealth : EntityCurrentStats.CurrentHealth;
        EntityCurrentStats = EntityStartingStats;
        EntityCurrentStats = EntityCurrentStats.CalcuateAllChanges(_statModifiers, originalHealth);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerCombatController : EntityCombatController, IKnockbackable
{

    //Statics
    public static PlayerCombatController Instance { get; private set; }

    //Public fields
    [field: SerializeField] public bool ImmunityFramesActive { get; private set; } = false;
    [field: SerializeField] public bool Stunned { get; private set; } = false;

    public event Action<CombatDamageReport> OnPlayerCompleteDefenseReport;
    public event Action<CombatDamageReport> OnPlayerCombatReportAttack;

    //Local fields
    [SerializeField] InventoryObject _armorSlots;
    [SerializeReference] Weapon _currentHeldWeapon;

    [field: SerializeField] public ModifiableFloatStat AttackSpeed { get; private set; }

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

        OnPlayerCompleteDefenseReport += CheckReport;
        WorldTimeManager.OnGameTick += PassiveRegen;
        HotBarHandler.Instance.OnSelectItem += HotbarSelectedItem;
        HotBarHandler.Instance.OnDeselectItem += HotbarDeselectItem;
        RaycastHandler.Instance.OnRaycastFail += AttemptAttack;
        PlayerMovementController.Instance.OnPlayerStartMove += UpdateDirection;
    }

    private void UpdateDirection()
    {
        if (FacingDirection == PlayerMovementController.Instance.CurrentDirection || PlayerMovementController.Instance.CurrentDirection == MoveDirection.Down || PlayerMovementController.Instance.CurrentDirection == MoveDirection.Up)
            return;

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
        Health.Heal(HealPerSecond);
    }


    public override bool TryReceiveAttack(Action<EntityCombatController> onAttack, out CombatDamageReport report)
    {
        report = null;
        if (ImmunityFramesActive)
            return false;

        StartCoroutine(StartImmunityFrames());
        return true;
    }

    IEnumerator StartImmunityFrames()
    {
        ImmunityFramesActive = true;
        yield return null;
        ImmunityFramesActive = false;
    }

    public async void Knockback(float distance, Vector2 direction)
    {
        Stunned = true;
        NewPlayerAnimationController.Instance.PlayStun();
        NewPlayerAnimationController.Instance._spriteRenderer.color = new Color(1, 0.5f, 0.5f, 1);
        PlayerMovementController.Instance.Rigidbody.velocity = direction * distance;
        while (PlayerMovementController.Instance.Rigidbody.velocity.magnitude > 0.3)
        {
            PlayerMovementController.Instance.UpdatePlayerPos();
            await Task.Delay(20);
        }
        PlayerMovementController.Instance.Rigidbody.velocity = Vector2.zero;
        NewPlayerAnimationController.Instance._spriteRenderer.color = Color.white;
        Stunned = false;
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




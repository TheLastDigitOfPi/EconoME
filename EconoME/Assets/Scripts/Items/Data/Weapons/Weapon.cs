using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class Weapon : Item
{
    public event Action<CombatDamageReport> OnAttackLand;

    [SerializeField] private int _weaponTier;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private WeaponDamage _startingDamage;
    [field: SerializeField] public WeaponDamage CurrentDamage { get; private set; }
    [SerializeField] AttackObject _attackObjectPrefab;
    [SerializeField] private List<WeaponEnchantmentSO> Enchantments = new();
    [SerializeField] private List<WeaponEnchantment> ActiveEnchantments = new();
    [field: SerializeField] public bool Attacking { get; private set; } = false;
    public WeaponBase WeaponBase { get { return ItemBase as WeaponBase; } }
    public Weapon(WeaponBase itemType, AttackObject attackObjectPrefab) : base(itemType)
    {
        _weaponTier = itemType.WeaponTier;
        _startingDamage = itemType.Damage;
        ActiveEnchantments = new();
        Enchantments = itemType.Enchantments;
        _attackSpeed = itemType.AttackSpeed;
        Stacksize = 1;
        _attackObjectPrefab = attackObjectPrefab;
    }

    public Weapon(){ }

    public Weapon(Weapon other) : base(other)
    {
        _weaponTier = other._weaponTier;
        _startingDamage = other._startingDamage;
        Enchantments = other.Enchantments;
        _attackSpeed = other._attackSpeed;
        _attackObjectPrefab = other._attackObjectPrefab;
    }

    public override Item Duplicate()
    {
        return new Weapon(this);
    }

    public virtual void Attack(PlayerCombatController owner)
    {
        if (Attacking || _attackObjectPrefab == null)
            return;
        Attacking = true;
        owner.StartCoroutine(UseAttack());
        IEnumerator UseAttack()
        {
            var animTime = NewPlayerAnimationController.Instance.GetAnimationTime(WeaponBase.AttackAnimation);
            Debug.Log("Attack called on weapon");
            NewPlayerAnimationController.Instance.TryPlayAttackAnimation(WeaponBase.AttackAnimation, _attackSpeed);
            var attackObject = UnityEngine.Object.Instantiate(_attackObjectPrefab);
            var offset = PlayerCombatController.Instance.FacingDirection == MoveDirection.Right? Vector3.right * 0.6f + Vector3.up * 0.2f: Vector3.left * 0.6f + Vector3.up * 0.2f;
            attackObject.Initialize(this, owner, offset);
            HeldItemHandler.Instance.StartProgress(animTime / _attackSpeed);
            yield return new WaitForSeconds(animTime / _attackSpeed);
            Attacking = false;
        }
    }

    public void DoAttack(EntityCombatController target)
    {
        Debug.Log("Tried to heal" + target.name + " for -10 health");
        target.Health.Damage(10);
    }

    public void AttackLanded(CombatDamageReport enemyHit)
    {
        OnAttackLand?.Invoke(enemyHit);
    }

    [SerializeField] List<AttackStatChangeInstance> _statModifiers = new();
    public void AddStatChange(AttackStatChangeInstance modifier)
    {
        _statModifiers.Add(modifier);
        UpdateAttack();
    }

    public void RemoveStatChange(AttackStatChangeInstance modifier)
    {
        _statModifiers.Remove(modifier);
        UpdateAttack();
    }

    void UpdateAttack()
    {
        CurrentDamage = _startingDamage;
        CurrentDamage = CurrentDamage.CalculateAllChanges(_statModifiers);
    }

    public void Unequip()
    {
        Debug.Log("Unequiping Weapon");
        foreach (var enchant in ActiveEnchantments)
        {
            enchant.OnUnequip();
        }
        ActiveEnchantments.Clear();
    }

    public void Equip(EntityCombatController owner)
    {
        Debug.Log("Equiping Weapon");
        ActiveEnchantments = new();
        _statModifiers.Clear();
        ActiveEnchantments.Clear();
        UpdateAttack();
        foreach (var enchantmentSO in Enchantments)
        {
            if (!enchantmentSO.TryGetEnchantment(owner, this, out var enchant))
                continue;
            ActiveEnchantments.Add(enchant);
            enchant.OnEquip();
        }
    }
}
public abstract class WeaponEnchantmentSO : ScriptableObject
{
    public abstract bool TryGetEnchantment(EntityCombatController owner, Weapon attack, out WeaponEnchantment enchantment);
}


public abstract class DamageInstance
{
    public abstract WeaponDamage CalculateDamage(WeaponDamage damage);
}

public class TrueDamageInstance : DamageInstance
{
    public override WeaponDamage CalculateDamage(WeaponDamage damage)
    {
        return damage;
    }
}

[System.Serializable]
public struct WeaponDamage
{
    public float TrueDamage;
    public float PhysicalDamage;
    public float RangedDamage;
    public float MagicalDamage;
    [SerializeField] public List<StatusEffectSO> StatusEffects;
    public WeaponDamage CalculateAllChanges(List<AttackStatChangeInstance> changes)
    {
        foreach (var modifier in changes)
        {
            CalculateBaseStatChange(modifier);
        }
        foreach (var modifier in changes)
        {
            CalculateSecondStatChange(modifier);
        }
        return this;
    }

    public void CalculateBaseStatChange(AttackStatChangeInstance statChange)
    {
        CalculateBaseChange(statChange.PhysicalDamageChanges, ref PhysicalDamage);
        CalculateBaseChange(statChange.MagicalDamageChanges, ref MagicalDamage);
        CalculateBaseChange(statChange.TrueDamageChanges, ref TrueDamage);
        CalculateBaseChange(statChange.RangedDamageChanges, ref RangedDamage);

        void CalculateBaseChange(StatChanger stat, ref float changingStat)
        {
            changingStat += (stat.BasePercentChange / 100f) * changingStat;
            changingStat += stat.BaseAdditionChange;
        }
    }

    public void CalculateSecondStatChange(AttackStatChangeInstance statChange)
    {
        CalculateSecondChange(statChange.PhysicalDamageChanges, ref PhysicalDamage);
        CalculateSecondChange(statChange.MagicalDamageChanges, ref MagicalDamage);
        CalculateSecondChange(statChange.TrueDamageChanges, ref TrueDamage);
        CalculateSecondChange(statChange.RangedDamageChanges, ref RangedDamage);

        void CalculateSecondChange(StatChanger stat, ref float changingStat)
        {
            changingStat += (stat.PerecentChange / 100f) * changingStat;
            changingStat += stat.AddtionChange;
        }
    }
}


public abstract class StatusEffectSO : ScriptableObject
{
    [field: SerializeField] public float Duration { get; protected set; }
    [field: SerializeField] public float Intensity { get; protected set; }
    public abstract StatusEffect GetStatusEffect();
}

[Serializable]
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
public struct AttackStatChangeInstance : IEquatable<AttackStatChangeInstance>
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
{
    public StatChanger PhysicalDamageChanges;
    public StatChanger MagicalDamageChanges;
    public StatChanger TrueDamageChanges;
    public StatChanger RangedDamageChanges;

    public bool Equals(AttackStatChangeInstance other)
    {
        return other.PhysicalDamageChanges == this.PhysicalDamageChanges
            && other.MagicalDamageChanges == this.MagicalDamageChanges
            && other.TrueDamageChanges == this.TrueDamageChanges
            && other.RangedDamageChanges == this.RangedDamageChanges;
    }

    public static bool operator ==(AttackStatChangeInstance c1, AttackStatChangeInstance c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(AttackStatChangeInstance c1, AttackStatChangeInstance c2)
    {
        return !c1.Equals(c2);
    }

    public static AttackStatChangeInstance operator +(AttackStatChangeInstance first, AttackStatChangeInstance second)
    {
        AttackStatChangeInstance result = new();
        result.PhysicalDamageChanges = first.PhysicalDamageChanges + second.PhysicalDamageChanges;
        result.MagicalDamageChanges = first.MagicalDamageChanges + second.MagicalDamageChanges;
        result.TrueDamageChanges = first.TrueDamageChanges + second.TrueDamageChanges;
        result.RangedDamageChanges = first.RangedDamageChanges + second.RangedDamageChanges;
        return result;
    }

}
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class Weapon : Item
{
    [SerializeField] private int _weaponTier;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private WeaponDamage _damage;
    [SerializeField] private List<WeaponEnchantmentSO> Enchantments = new();
    [SerializeField] private List<WeaponEnchantment> ActiveEnchantments = new();
    [field: SerializeField] public bool Attacking { get; private set; } = false;
    public WeaponBase WeaponBase { get { return ItemBase as WeaponBase; } }
    public Weapon(WeaponBase itemType) : base(itemType)
    {
        _weaponTier = itemType.WeaponTier;
        _damage = itemType.Damage;
        ActiveEnchantments = new();
        Enchantments = itemType.Enchantments;
        _attackSpeed = itemType.AttackSpeed;
        Stacksize = 1;
    }

    public Weapon(Weapon other) : base(other)
    {
        _weaponTier = other._weaponTier;
        _damage = other._damage;
        Enchantments = other.Enchantments;
        _attackSpeed = other._attackSpeed;
    }

    public override Item Duplicate()
    {
        return new Weapon(this);
    }

    public virtual void Attack(PlayerCombatController owner)
    {
        if (Attacking)
            return;
        Attacking = true;
        owner.StartCoroutine(UseAttack());
        IEnumerator UseAttack()
        {
            var animTime = NewPlayerAnimationController.Instance.GetAnimationTime(WeaponBase.AttackAnimation);
            Debug.Log("Attack called on weapon");
            NewPlayerAnimationController.Instance.TryPlayAttackAnimation(WeaponBase.AttackAnimation, _attackSpeed);
            yield return new WaitForSeconds(animTime / _attackSpeed);
            Attacking = false;
        }
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
        ActiveEnchantments.Clear();
        foreach (var enchantmentSO in Enchantments)
        {
            if (!enchantmentSO.TryGetEnchantment(owner, out var enchant))
                continue;
            ActiveEnchantments.Add(enchant);
            enchant.OnEquip(owner);
        }
    }
}
public abstract class WeaponEnchantmentSO : ScriptableObject
{
    public abstract bool TryGetEnchantment(EntityCombatController owner, out WeaponEnchantment enchantment);
}

[Serializable]
public abstract class WeaponEnchantment
{
    protected EntityCombatController owner;
    public virtual void OnEquip(EntityCombatController armorHolder)
    {
        owner = armorHolder;
    }
    public virtual void OnUnequip()
    {
        owner = default;
    }
}
[System.Serializable]
public struct WeaponDamage
{
    public float TrueDamage;
    public float PhysicalDamage;
    public float RangedDamage;
}




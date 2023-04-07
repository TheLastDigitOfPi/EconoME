using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Base", menuName = "ScriptableObjects/Items/Weapons")]
public class WeaponBase : ItemBase
{
    [field: SerializeField] public int WeaponTier { get; private set; }
    [field: SerializeField] public WeaponDamage Damage { get; private set; }
    [field: SerializeField] public float AttackSpeed { get; private set; }
    [field: SerializeField] public List<WeaponEnchantmentSO> Enchantments { get; private set; }
    [field: SerializeField] public WeaponType WeaponType { get; private set; }
    [field: SerializeField] public AttackObject AttackObject { get; private set; }
    [field: SerializeField] public NewPlayerAnimationController.AttackAnimation AttackAnimation { get; private set; }
    public override Item CreateItem(int stackSize)
    {
        return new Weapon(this, AttackObject);
    }
}

public enum WeaponType
{
    LongSword,
    ShortSword,
    Bow
}


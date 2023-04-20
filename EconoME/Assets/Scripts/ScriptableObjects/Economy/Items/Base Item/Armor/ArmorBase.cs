using UnityEngine;
using System;
using System.Numerics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

[CreateAssetMenu(fileName = "New Armor Base", menuName = "ScriptableObjects/Items/Armor")]
public class ArmorBase : ItemBase
{
    [field: SerializeField] public ArmorType armorType { get; private set; }
    [field: SerializeField] public List<ArmorEnchantmentSO> Enchantments { get; private set; }

    public override Item CreateItem(int stackSize)
    {
        return new Armor(this);
    }
}
public enum ArmorType
{
    Helmet,
    Chestplate,
    Leggings,
    Boots
}
[Serializable]
public struct DefenseStatChangeInstance : IEquatable<DefenseStatChangeInstance>
{
    public StatChanger HealthChanges;
    public StatChanger PhysicalDefenseChanges;
    public StatChanger MagicalDefenseChanges;
    public StatChanger RangedDefenseChanges;
    public StatChanger TrueDefenseChanges;
    public StatChanger HPSChanges;

    public bool Equals(DefenseStatChangeInstance other)
    {
        return other.HealthChanges == this.HealthChanges
            && other.PhysicalDefenseChanges == this.PhysicalDefenseChanges
            && other.MagicalDefenseChanges == this.MagicalDefenseChanges
            && other.RangedDefenseChanges == this.RangedDefenseChanges
            && other.TrueDefenseChanges == this.TrueDefenseChanges
            && other.HPSChanges == this.HPSChanges;
    }

    public static bool operator ==(DefenseStatChangeInstance c1, DefenseStatChangeInstance c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(DefenseStatChangeInstance c1, DefenseStatChangeInstance c2)
    {
        return !c1.Equals(c2);
    }

    public static DefenseStatChangeInstance operator +(DefenseStatChangeInstance first, DefenseStatChangeInstance second)
    {
        DefenseStatChangeInstance result = new();
        result.HealthChanges = first.HealthChanges + second.HealthChanges;
        result.PhysicalDefenseChanges = first.PhysicalDefenseChanges + second.PhysicalDefenseChanges;
        result.MagicalDefenseChanges = first.MagicalDefenseChanges + second.MagicalDefenseChanges;
        result.RangedDefenseChanges = first.RangedDefenseChanges + second.RangedDefenseChanges;
        result.TrueDefenseChanges = first.TrueDefenseChanges + second.TrueDefenseChanges;
        return result;
    }

}
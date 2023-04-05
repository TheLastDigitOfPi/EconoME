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
    public StatCalulation HealthChanges;
    public StatCalulation PhysicalDefenseChanges;
    public StatCalulation MagicalDefenseChanges;
    public StatCalulation RangedDefenseChanges;
    public StatCalulation TrueDefenseChanges;
    public StatCalulation HPSChanges;

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
[Serializable]
public struct StatCalulation : IEquatable<StatCalulation>
{
    public float BasePercentChange;
    public float BaseAdditionChange;
    public float PerecentChange;
    public float AddtionChange;

    public bool Equals(StatCalulation other)
    {
        return other.BasePercentChange == this.BasePercentChange
            && other.BaseAdditionChange == this.BaseAdditionChange
            && other.PerecentChange == this.PerecentChange
            && other.AddtionChange == this.AddtionChange;
    }

    public static bool operator ==(StatCalulation c1, StatCalulation c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(StatCalulation c1, StatCalulation c2)
    {
        return !c1.Equals(c2);
    }

    public static StatCalulation operator +(StatCalulation first, StatCalulation second)
    {
        StatCalulation result = new();
        result.BasePercentChange = first.BasePercentChange + second.BasePercentChange;
        result.BaseAdditionChange = first.BaseAdditionChange + second.BaseAdditionChange;
        result.PerecentChange = first.PerecentChange + second.PerecentChange;
        result.AddtionChange = first.AddtionChange + second.AddtionChange;
        return result;
    }
}
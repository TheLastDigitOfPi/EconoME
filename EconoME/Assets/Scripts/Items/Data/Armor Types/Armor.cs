using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Armor : Item
{
    [field: SerializeField] public ArmorType ArmorType { get; private set; }
    [field: SerializeField] public List<ArmorEnchantmentSO> Enchantments { get; private set; }
    [field: SerializeReference] public List<ArmorEnchantment> ActiveEnchantments = new();
    public Armor(Armor other) : base(other)
    {
        ArmorType = other.ArmorType;
        Enchantments = other.Enchantments;
    }

    public Armor(ArmorBase itemBase) : base(itemBase)
    {
        ArmorType = itemBase.armorType;
        Enchantments = itemBase.Enchantments;
    }

    public override Item Duplicate()
    {
        return new Armor(this);
    }

    public void Unequip()
    {
        Debug.Log("Unequiping Armor Piece");
        foreach (var enchant in ActiveEnchantments)
        {
            enchant.OnUnequip();
        }
        ActiveEnchantments.Clear();
    }

    public void Equip(EntityCombatController owner)
    {
        Debug.Log("Equiping Armor Piece");
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


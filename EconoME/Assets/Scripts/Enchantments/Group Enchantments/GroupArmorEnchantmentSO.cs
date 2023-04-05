using System.Collections.Generic;
using UnityEngine;
public class GroupArmorEnchantmentSO : ScriptableObject
{
    [SerializeField] ArmorEnchantmentSO[] CustomEnchantments;

    public ArmorEnchantment[] GetEnchantments(EntityCombatController owner)
    {
        List<ArmorEnchantment> enchants = new();
        foreach (var enchantment in CustomEnchantments)
        {
            if(enchantment.TryGetEnchantment(owner, out var validEnchant))
                enchants.Add(validEnchant);
        }
        return enchants.ToArray();
    }
}

public class GroupArmorEnchantment : ArmorEnchantment
{
    [SerializeField] ArmorEnchantment[] CustomEnchantments;
    public override void OnEquip(EntityCombatController owner)
    {
        foreach (var enchantment in CustomEnchantments)
        {
            enchantment.OnEquip(owner);
        }
    }

    public override void OnUnequip()
    {
        foreach (var enchantment in CustomEnchantments)
        {
            enchantment.OnUnequip();
        }
    }
}

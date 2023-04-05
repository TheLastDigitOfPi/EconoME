using UnityEngine;
public abstract class ArmorEnchantmentSO : ScriptableObject
{
    public abstract bool TryGetEnchantment(EntityCombatController owner, out ArmorEnchantment enchantment);
}

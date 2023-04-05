using System;

[Serializable]
public abstract class ArmorEnchantment
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

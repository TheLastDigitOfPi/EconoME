using System;

[Serializable]
public abstract class WeaponEnchantment
{
    protected EntityCombatController owner;
    protected Weapon attack;

    public WeaponEnchantment(EntityCombatController owner, Weapon attack)
    {
        this.owner = owner;
        this.attack = attack;
    }
    public abstract void OnEquip();
    public abstract void OnUnequip();
}

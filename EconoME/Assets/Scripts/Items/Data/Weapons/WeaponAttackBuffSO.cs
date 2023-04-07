using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Attack Buff", menuName = "ScriptableObjects/Enchants/Weapons/Attack Buff")]
public class WeaponAttackBuffSO : WeaponEnchantmentSO
{
    [SerializeField] AttackStatChangeInstance attackChanges;
    public override bool TryGetEnchantment(EntityCombatController owner, Weapon attack, out WeaponEnchantment enchantment)
    {
        enchantment = new WeaponAttackBuff(owner, attack, attackChanges);
        return owner != null;
    }
}

public class WeaponAttackBuff : WeaponEnchantment
{
    AttackStatChangeInstance _attackChange;

    public WeaponAttackBuff(EntityCombatController owner, Weapon attack, AttackStatChangeInstance attackChange) : base(owner, attack)
    {
        _attackChange = attackChange;
    }

    public override void OnEquip()
    {
        attack.AddStatChange(_attackChange);
    }

    public override void OnUnequip()
    {
        attack.RemoveStatChange(_attackChange);
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "New Defense Enchantment", menuName = "ScriptableObjects/Enchants/Armor/Flat Defense Enchantment")]
public class DefenseEnchantmentSO : ArmorEnchantmentSO
{
    [SerializeField] DefenseStatChangeInstance StatChanges;
    public override bool TryGetEnchantment(EntityCombatController owner, out ArmorEnchantment enchant)
    {
        enchant = new DefenseEnchantment(StatChanges);
        return owner != null;
    }
}
public class DefenseEnchantment : ArmorEnchantment
{
    [SerializeField] DefenseStatChangeInstance StatChanges;
    public override void OnEquip(EntityCombatController owner)
    {
        base.OnEquip(owner);
        if(owner is not IDefenseHolder)
            return;
        var defender = owner as IDefenseHolder;
        defender.AddStatChange(StatChanges);
    }

    public DefenseEnchantment(DefenseStatChangeInstance changes)
    {
        StatChanges = changes;
    }

    private void AddDefenses(CombatDamageReport report)
    {
        FlatDefenseModifier modifier = new FlatDefenseModifier(StatChanges);
        report.AddReportModifier(modifier);
    }

    public override void OnUnequip()
    {
        if(owner is not IDefenseHolder)
            return;
        var defender = owner as IDefenseHolder;
        defender.RemoveStatChange(StatChanges);
    }
}

[System.Serializable]
public class FlatDefenseModifier : CombatReportModifier
{
    [SerializeField] DefenseStatChangeInstance StatChanges;
    public FlatDefenseModifier(DefenseStatChangeInstance changes)
    {
        Priority = CombatReportPriority.FlatStatChanges;
        StatChanges = changes;
    }
    public override void ApplyModifier(CombatDamageReport report)
    {
        report.DefenseChanges += StatChanges;
    }
}
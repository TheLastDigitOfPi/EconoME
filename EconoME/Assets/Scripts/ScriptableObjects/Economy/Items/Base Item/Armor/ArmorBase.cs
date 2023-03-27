using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ArmorBase : ItemBase
{
    [field: SerializeField] public ArmorType armorType { get; private set; }
    [field: SerializeField] public List<ArmorAttributeBase> Attributes { get; private set; }

    public override Item CreateItem(int stackSize)
    {
        return new Armor(this);
    }

    public List<ArmorAttribute> GetAttributes()
    {
        var newList = new List<ArmorAttribute>();
        foreach (var item in Attributes)
        {
            newList.Add(item.Attribute);
        }
        return newList;
    }

}
public enum ArmorType
{
    Helmet,
    Chestplate,
    Leggings,
    Boots
}

public abstract class ArmorAttributeBase : ScriptableObject
{
    [SerializeField] public abstract ArmorAttribute Attribute { get; set; }
}


public abstract class ArmorDefensesBase : ArmorAttributeBase
{

}

public abstract class ArmorAttribute
{
    public abstract CombatEntityDefenses AddAttribute(CombatEntityDefenses defenses);
    public abstract void OnEquip();
    public abstract void OnUnequip();

}

public class ArmorDefenses : ArmorAttribute
{
    public override CombatEntityDefenses AddAttribute(CombatEntityDefenses defenses)
    {
        throw new NotImplementedException();
    }

    public override void OnEquip()
    {

    }
    public override void OnUnequip()
    {
        throw new NotImplementedException();
    }
}

public class ArmorDamageMitagation : ArmorAttribute
{
    public override CombatEntityDefenses AddAttribute(CombatEntityDefenses defenses)
    {
        throw new NotImplementedException();
    }

    public override void OnEquip()
    {
    }

    public override void OnUnequip()
    {
    }
}

public class ArmorSpecialEffectExample : ArmorAttribute
{
    public override CombatEntityDefenses AddAttribute(CombatEntityDefenses defenses)
    {
        throw new NotImplementedException();
    }

    private async void OnDefense(CombatDamageReport report)
    {
        if (report.BlockedAttack)
        {
            await Task.Delay(5000);
        }
    }
    public override void OnEquip()
    {
        PlayerCombatController.Instance.OnPlayerCombatReportDefense += OnDefense;
    }

    public override void OnUnequip()
    {
        PlayerCombatController.Instance.OnPlayerCombatReportDefense -= OnDefense;
    }
}
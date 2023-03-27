using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Armor : Item
{
    [field: SerializeField] public ArmorType ArmorType { get; private set; }
    [field: SerializeField] public List<ArmorAttribute> Attributes { get; private set; }
    public Armor(Armor other) : base(other)
    {
        ArmorType = other.ArmorType;
        Attributes = other.Attributes;
    }

    public Armor(ArmorBase itemBase) : base(itemBase)
    {
        ArmorType = itemBase.armorType;
        Attributes = itemBase.GetAttributes();
    }

    public void OnEquip()
    {
        foreach (var item in Attributes)
        {

        }
    }
}


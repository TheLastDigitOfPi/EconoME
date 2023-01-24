using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[Serializable]
public class Item
{
    //Base Class for items that can be placed into inventory

    //Public Data

    public ItemBase ItemBase;
    public string ItemName { get { return ItemBase.ItemName; } }
    public Sprite Icon { get { return ItemBase.Icon; } }
    public string IconPath { get { return ItemBase.IconPath; } }

    public void SetIcon(Sprite icon, string iconPath)
    {
    }

    public int IndividualItemWeight { get { return ItemBase.Weight; } }
    public int StackWeight { get { return Stacksize * IndividualItemWeight; } }

    public int Stacksize = -1;
    public Item(ItemBase itemBase, int stackSsize = 1)
    {
        ItemBase = itemBase;
        this.Stacksize = stackSsize;
    }

    public Item()
    {

    }

    public Item(Item other)
    {
        if (other == null)
        {
            return;
        }
        ItemBase = other.ItemBase;
        Stacksize = other.Stacksize;
    }

    public virtual Item Duplicate()
    {
        return new Item(this);
    }

    public virtual bool IsValid(out string Error)
    {
        if (Stacksize < 1) { Error = "Invalid Stack Size"; return false; }
        if (ItemBase == null) { Error = "Itembase was null"; return false; }
        Error = default;
        return true;
    }

    public bool SameBase(Item other)
    {
        if (other.ItemBase != this.ItemBase)
            return false;

        return true;
    }
}


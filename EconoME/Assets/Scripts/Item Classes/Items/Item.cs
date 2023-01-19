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

    //Semi-Public Data. TODO: Make Constructors

    public ItemScriptableObject ItemBase;
    public string ItemName { get { return ItemBase.ItemName; } }
    public Sprite Icon
    {
        get
        {
            if (UseNewIcon)
                return _icon;
            if(ItemBase == null)
                return null;
            return ItemBase.Icon;
        }
    }
    bool UseNewIcon = false;
    Sprite _icon;
    string _iconPath;
    public string IconPath
    {
        get
        {
            if (UseNewIcon)
                return _iconPath;
            return ItemBase.IconPath;
        }
    }

    public void SetIcon(Sprite icon, string iconPath)
    {
        _icon = icon;
        _iconPath = iconPath;
        UseNewIcon = true;
    }

    public ItemType itemType { get { return ItemBase? ItemBase.ItemType: null; } }
    public int Weight { get { return ItemBase.Weight; } }
    public int CurrentWeight { get { return Stacksize * Weight; } set { } }

    public int Stacksize = -1;
    public Item(ItemScriptableObject itemBase, int stackSsize = 1)
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
}


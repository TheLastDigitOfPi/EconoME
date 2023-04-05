using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

[Serializable]
public class Item
{
    //Base Class for items that can be placed into inventory

    //Event Data
    public event Action OnItemUpdate;

    //Public Data
    [field: SerializeField] public ItemBase ItemBase { get; private set; }
    [SerializeField] int _stackSize = -1;
    public int Stacksize { get { return _stackSize; } set { _stackSize = value; OnItemUpdate?.Invoke(); } }
    //Helpers
    public string ItemName { get { return ItemBase.ItemName; } }
    public ItemIcon ForegroundIcon { get { return ItemBase.ForegroundIcon; } }
    public ItemIcon BackgroundIcon { get { return ItemBase.BackgroundIcon; } }
    public ItemIcon EffectIcon { get { return ItemBase.EffectIcon; } }
    public int IndividualItemWeight { get { return ItemBase.Weight; } }
    public int StackWeight { get { return Stacksize * IndividualItemWeight; } }

    public Item(ItemBase itemBase, int stackSsize = 1)
    {
        ItemBase = itemBase;
        this.Stacksize = stackSsize;
    }

    public void ResetEvents()
    {
        OnItemUpdate = null;
    }

    /// <summary>
    /// Duplicate this item.
    /// </summary>
    /// <param name="other"></param>
    public Item(Item other)
    {
        if (other == null)
        {
            return;
        }
        ItemBase = other.ItemBase;
        Stacksize = other.Stacksize;
    }

    public Item()
    {

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

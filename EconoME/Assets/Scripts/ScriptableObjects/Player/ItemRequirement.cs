using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ItemRequirement
{
    public string ItemName;
    public int ItemCount;
    public ItemType itemType;
    public ItemType[] InvalidItemTypes = new ItemType[0];
    public ItemTypeContainer[] InvalidItemGroups = new ItemTypeContainer[0];

    public ItemRequirement(ItemType itemType, string itemName = "", int itemCount = 1)
    {
        ItemName = itemName;
        ItemCount = itemCount;
        this.itemType = itemType;
    }

    public ItemRequirement(ItemType itemType, string itemName, int itemCount, ItemType[] invalidItemTypes, ItemTypeContainer[] invalidItemGroups)
    {
        ItemName = itemName;
        ItemCount = itemCount;
        this.itemType = itemType;
        InvalidItemTypes = invalidItemTypes;
        InvalidItemGroups = invalidItemGroups;
    }

    public bool isValidItem(Item other)
    {
        if (other == null || other.Weight == 0)
        {
            return false;
        }

        for (int i = 0; i < InvalidItemTypes.Length; i++)
        {
            if (itemType == InvalidItemTypes[i])
            {
                return false;
            }
        }

        for (int i = 0; i < InvalidItemGroups.Length; i++)
        {
            for (int j = 0; j < InvalidItemGroups[i].Types.Length; j++)
            {
                if (itemType == InvalidItemGroups[i].Types[j])
                {
                    return false;
                }
            }
        }

        if (ItemCount > 1)
        {
            if (other.Stacksize < ItemCount) { return false; }
        }
        if (ItemName != null && ItemName != "")
        {
            if (other.ItemName != ItemName) { return false; }
        }

        return true;
    }

    public void Reset()
    {
        ItemName = "";
        ItemCount = 0;
    }
}

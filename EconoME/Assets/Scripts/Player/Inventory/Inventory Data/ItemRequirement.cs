using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ItemRequirement
{
    [SerializeField] private string ItemName;
    [SerializeField] private int _requiredItemCount;
    [SerializeField] private ItemType _requiredItemType;
    [SerializeField] private ItemType[] _invalidItemTypes = new ItemType[0];

    public bool isValidItem(Item other)
    {
        if (other == null || other.IndividualItemWeight == 0) { return false; }

        for (int i = 0; i < _invalidItemTypes.Length; i++)
        {
            if (_requiredItemType == _invalidItemTypes[i])
            {
                return false;
            }
        }

        if(_requiredItemType != ItemType.Any)
            if (other.ItemBase.ItemType != _requiredItemType) { return false; }
        
        foreach (var invalidType in _invalidItemTypes)
        {
            if(other.ItemBase.ItemType == invalidType)
                return false;
        }

        if (_requiredItemCount > 1) { if (other.Stacksize < _requiredItemCount) { return false; } }
        if (ItemName != null && ItemName != "")
        {
            if (other.ItemName != ItemName) { return false; }
        }

        return true;
    }

    public void Reset()
    {
        ItemName = "";
        _requiredItemCount = 0;
    }
}

public enum ItemType
{
    Any,
    Tool,
    Resource,
    Armor
}

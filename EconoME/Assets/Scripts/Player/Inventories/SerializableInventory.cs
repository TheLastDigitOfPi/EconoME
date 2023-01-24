using System;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class SerializableInventory
{
    //An inventory represents a set of slots and their correlated item
    [field: SerializeField] public string InventoryName { get; private set; }
    [SerializeReference] public ItemSlot[] ItemSlots;
    public SerializableInventory(string inventoryName, ItemSlot[] items)
    {
        this.InventoryName = inventoryName;
        this.ItemSlots = items;
    }
}

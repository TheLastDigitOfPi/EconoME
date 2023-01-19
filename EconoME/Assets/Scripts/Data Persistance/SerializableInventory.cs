using System;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class SerializableInventory
{
    public string inventoryName;
    [SerializeReference] public Item[] items;
    public SerializableInventory(string inventoryName, Item[] items)
    {
        this.inventoryName = inventoryName;
        this.items = items;
    }
}
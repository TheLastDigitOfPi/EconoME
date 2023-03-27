using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;


public class InventoryManager
{
    public static bool AddToInventoryGroup(InventorySlotHandler[] slots, Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].AddItem(item, out _))
            {
                return true;
            }
        }
        return false;
    }
    public static int FindItems(Item itemSearch, InventorySlotHandler[] SlotsToLookThrough, bool stopAtItemAmount = false)
    {
        int foundItemsCount = 0;

        for (int i = 0; i < SlotsToLookThrough.Length; i++)
        {
            if (!SlotsToLookThrough[i].ItemSlot.HasItem) { continue; }
            if (SlotsToLookThrough[i].ItemSlot.ItemBase.ItemName == itemSearch.ItemName)
            {
                foundItemsCount += SlotsToLookThrough[i].ItemSlot.StackSize;
                if (stopAtItemAmount) { if (foundItemsCount >= itemSearch.Stacksize) { return 0; } }
            }
        }
        return foundItemsCount;
    }

    public static bool RemoveItems(List<Item> itemsToRemove, InventorySlotHandler[] SlotsToLookThrough)
    {
        for (int i = 0; i < itemsToRemove.Count; i++)
        {
            if (FindItems(itemsToRemove[i], SlotsToLookThrough, true) > 0) { return false; }
        }

        for (int x = 0; x < itemsToRemove.Count; x++)
        {
            Item itemToRemove = itemsToRemove[x];

            int RemainingItems = itemToRemove.Stacksize;

            //Removed items from resource slots
            for (int i = 0; i < SlotsToLookThrough.Length; i++)
            {
                InventorySlotHandler CuurentSlot = SlotsToLookThrough[i];
                if (!CuurentSlot.ItemSlot.HasItem) { continue; }
                if (CuurentSlot.ItemSlot.ItemBase.ItemName == itemToRemove.ItemName)
                {
                    //If stacksize is greater/= to what we need, we are done
                    if (CuurentSlot.ItemSlot.StackSize >= RemainingItems)
                    {
                        CuurentSlot.ItemSlot.StackSize -= RemainingItems;
                        //CuurentSlot.UpdateSlot();
                        return true;
                    }
                    RemainingItems -= CuurentSlot.ItemSlot.StackSize;
                    CuurentSlot.RemoveItem();
                }
            }

        }
        Debug.LogWarning("Failed to remove items even after checking they were there, bruh");
        return false;
    }

    public static bool RemoveItems(List<DefinedScriptableItem> itemsToRemove, InventorySlotHandler[] SlotsToLookThrough)
    {
        List<Item> items = new();
        for (int i = 0; i < itemsToRemove.Count; i++)
        {
            items.Add(itemsToRemove[i].item.CreateItem(itemsToRemove[i].stacksize));
        }
        return (RemoveItems(items, SlotsToLookThrough));
    }

    public static bool RemoveItems(Item itemToRemove, InventorySlotHandler[] SlotsToLookThrough)
    {
        if (FindItems(itemToRemove,SlotsToLookThrough, true) < itemToRemove.Stacksize) { return false; }

        int RemainingItems = itemToRemove.Stacksize;

        //Removed items from resource slots
        for (int i = 0; i < SlotsToLookThrough.Length; i++)
        {
            if (!SlotsToLookThrough[i].ItemSlot.HasItem) { continue; }
            if (SlotsToLookThrough[i].ItemSlot.ItemBase.ItemName == itemToRemove.ItemName)
            {
                //If stacksize is greater/= to what we need, we are done
                if (SlotsToLookThrough[i].ItemSlot.StackSize >= RemainingItems)
                {
                    SlotsToLookThrough[i].ItemSlot.StackSize -= RemainingItems;
                    //SlotsToLookThrough[i].UpdateSlot();
                    return true;
                }
                RemainingItems -= SlotsToLookThrough[i].ItemSlot.StackSize;
                SlotsToLookThrough[i].RemoveItem();
            }
        }
        Debug.LogWarning("Failed to remove items even after checking they were there, bruh");
        return false;
    }


}
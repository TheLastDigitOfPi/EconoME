using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Inventory Object", menuName = "ScriptableObjects/Player/Inventory Object")]
public class InventoryObject : ScriptableObject
{
    //Data is separated from ScriptableObject so that it can be easier serialized, possibly changed in future if found better way to serialize and deserialize Scriptable Objects
    [field: SerializeField] public SerializableInventory Data { get; private set; }
    [field: SerializeField] public IntVariable MaxSlots { get; private set; }
    [field: SerializeField] public ItemRequirement ItemRequirement { get; private set; }
    [field: SerializeField] public IntVariable MaxInventorySlotWeight { get; private set; }

    /// <summary>
    /// Attempts to add item to this inventory
    /// </summary>
    /// <param name="item"></param>
    /// <param name="AddToEmpty">Should this attempt to add to empty slots</param>
    /// <param name="CheckStacks">Should this attempt to add to stacks of same item </param>
    /// <returns>Returns true if the item was fully added to the found inventory</returns>
    public bool AddItem(Item item, bool AddToEmpty = true, bool CheckStacks = true)
    {
        if (!ItemRequirement.isValidItem(item))
            return false;

        bool ItemFullyAdded = false;
        if (CheckStacks)
            AddItemToValidStacks();

        if (AddToEmpty && !ItemFullyAdded)
            AddItemToEmptySlots();

        return ItemFullyAdded;


        void AddItemToValidStacks()
        {
            for (int i = 0; i < Data.ItemSlots.Length; i++)
            {
                //If cannot be stacked on, try next slot
                if (!Data.ItemSlots[i].HasItem || Data.ItemSlots[i].IsFull || !Data.ItemSlots[i].SameBase(item))
                    continue;

                var currentSlot = Data.ItemSlots[i];
                //Shove rest of items in if they fit
                if (currentSlot.RemainingStackRoom >= item.Stacksize)
                {
                    currentSlot.StackSize += item.Stacksize;
                    ItemFullyAdded = true;
                    return;
                }
                //Otherwise fit as many as we can
                item.Stacksize -= currentSlot.RemainingStackRoom;
                currentSlot.StackSize = currentSlot.MaxItems;

            }
        }

        void AddItemToEmptySlots()
        {
            for (int i = 0; i < Data.ItemSlots.Length; i++)
            {
                //If there is an item try next slot
                if (Data.ItemSlots[i].HasItem)
                    continue;

                //Set new slot to the item
                var currentSlot = Data.ItemSlots[i];
                currentSlot.Item = item.Duplicate();
                //If the item overloaded the slot, update the item and fix the slot
                if (currentSlot.RemainingStackRoom < 0)
                {
                    item.Stacksize -= currentSlot.MaxItems;
                    currentSlot.StackSize = currentSlot.MaxItems;
                    continue;
                }
                ItemFullyAdded = true;
                return;
            }
        }
    }
    /// <summary>
    /// Attempts to Swap the item given with the current item in the slot, swappeditem will return the item wanted to be swapped if failed, otherwise the item wanted to be swapped with
    /// </summary>
    /// <param name="itemToSwap"></param>
    /// <param name="swappedItem">The item swapped (item in the slot) if successful, otherwise the original item attempted to be swapped</param>
    /// <param name="slotNum"></param>
    /// <returns>Returns true if able to swap the item</returns>
    internal bool SawpItem(Item itemToSwap, out Item swappedItem, int slotNum)
    {
        var itemSlot = Data.ItemSlots[slotNum];
        Item slotOriginalItem = itemSlot.ItemCopy;
        itemSlot.Item = null;

        if (ItemRequirement.isValidItem(itemToSwap))
        {
            itemSlot.Item = itemToSwap;
            swappedItem = slotOriginalItem;
            return true;
        }
        swappedItem = null;
        itemSlot.Item = slotOriginalItem;
        return false;

    }
    /// <summary>
    /// Attempt to move item to the given slot
    /// </summary>
    /// <param name="item"></param>
    /// <param name="slotNum"></param>
    /// <param name="PartialAddition">Returns true if only a part of the item was able to fit</param>
    /// <returns>Returns true if the whole item could fit, otherwise outputs whether a partial addition was made in the out param</returns>
    public bool AddItemToSlot(Item item, int slotNum, out bool PartialAddition)
    {
        PartialAddition = false;
        if (!ItemRequirement.isValidItem(item))
            return false;

        var currentSlot = Data.ItemSlots[slotNum];
        //If there is no item, attempt to directly add to slot
        if (!Data.ItemSlots[slotNum].HasItem)
        {
            //Set new slot to the item
            currentSlot.Item = item.Duplicate();
            //If placed more than possible, then we add the partial amount
            if (currentSlot.RemainingStackRoom < 0)
            {
                item.Stacksize -= currentSlot.MaxItems;
                currentSlot.StackSize = currentSlot.MaxItems;
                PartialAddition = true;
                return false;
            }
            return true;
        }

        //Otherwise check if slot item is the same and not full
        if (Data.ItemSlots[slotNum].IsFull || !Data.ItemSlots[slotNum].SameBase(item))
            return false;

        //Shove rest of items in if they fit
        if (currentSlot.RemainingStackRoom >= item.Stacksize)
        {
            currentSlot.StackSize += item.Stacksize;
            return true;
        }
        //Otherwise fit as many as we can
        item.Stacksize -= currentSlot.RemainingStackRoom;
        currentSlot.StackSize = currentSlot.MaxItems;
        PartialAddition = true;
        return false;
    }
    /// <summary>
    /// Removes the item from the given slot and outputs the item that was removed
    /// </summary>
    /// <param name="slotNum"></param>
    /// <param name="itemInSlot">The item that was removed</param>
    public void RemoveItemFromSlot(int slotNum, out Item itemInSlot)
    {
        itemInSlot = Data.ItemSlots[slotNum].ItemCopy;
        Data.ItemSlots[slotNum].Item = null;
    }

    public void InitializeData()
    {
        if (!MaxSlots) { Debug.LogWarning("Set the Max Slots Amount for inventory: " + name); return; }

        if (Data.ItemSlots == null)
            Data.ItemSlots = new ItemSlot[MaxSlots.Value];

        //Ensure the item count and current slot count are valid
        if (Data.ItemSlots.Length != MaxSlots.Value)
        {
            if (Data.ItemSlots.Length > MaxSlots.Value)
                Debug.LogWarning("Slot Length is Higher than Max Slots amount, possible data loss ahead");

            //Move old data over if there, otherwise initialize new slot
            ItemSlot[] newSlotsArray = new ItemSlot[MaxSlots.Value];
            for (int i = 0; i < MaxSlots.Value; i++)
            {
                if (i < Data.ItemSlots.Length && Data.ItemSlots[i] != null)
                {
                    newSlotsArray[i] = Data.ItemSlots[i];
                    continue;
                }
                newSlotsArray[i] = new ItemSlot(i, this);
            }
            Data.ItemSlots = newSlotsArray;
        }

        InitializeSlots();

        void InitializeSlots()
        {
            foreach (var slot in Data.ItemSlots)
            {
                slot.Initialize();
            }
        }
    }
}

using UnityEngine;
using System;

[Serializable]
public class InventorySlot
{
    public bool isEmpty
    {
        get
        {
            if (item == null) { return true; }
            if (item.ItemBase == null) { return true; }
            if (item.Stacksize < 1) { return true; }
            return false;
        }
    }

    public ItemRequirement itemRequirement;

    public InventoryObject InventoryGroup;
    public int slotIndex;

    public Item item { get { return InventoryGroup?.data?.items?[slotIndex]; } set { InventoryGroup.data.items[slotIndex] = value; } }

    public bool Active = true;
    [SerializeField] Sprite OriginalImage;
    public SlotType slotType;
    [SerializeField] IntVariable InventoryMaxSlotWeight;
    protected int MaxInventorySlotWeight { get { return InventoryMaxSlotWeight.Value; } }
    public enum SlotType
    {
        Backback,
        Hotbar,
        ResourceBag,
        Chest,
        Bank,
        Shop
    }

    public InventorySlot()
    {
        slotType = SlotType.Backback;
    }


    public virtual bool SetSlot(Item NewItem, InventorySlotHandler slotToUpdate)
    {
        bool success = TryAddItemToSlot(NewItem);
        UpdateSlot(slotToUpdate);
        return success;

        bool TryAddItemToSlot(Item NewItem)
        {
            if (itemRequirement != null)
                if (!itemRequirement.isValidItem(NewItem)) { return false; }
            if (isEmpty)
            {
                itemRequirement.ItemName = NewItem.ItemName;
                if (NewItem.Weight < 0) { item = NewItem; return true; }

                if (NewItem.CurrentWeight > MaxInventorySlotWeight)
                {
                    item = NewItem.Duplicate();
                    item.Stacksize = MaxInventorySlotWeight / item.Weight;
                    NewItem.Stacksize -= item.Stacksize;
                    return false;
                }

                item = NewItem.Duplicate();
                return true;
            }
            if (NewItem.Weight < 0) { return false; }
            if (item.CurrentWeight + item.Weight >= MaxInventorySlotWeight) { return false; }
            //New Item cannot fully fit into current item
            if (item.CurrentWeight + NewItem.CurrentWeight > MaxInventorySlotWeight)
            {
                NewItem.Stacksize -= (MaxInventorySlotWeight / item.Weight) - item.Stacksize;
                item.Stacksize = MaxInventorySlotWeight / item.Weight;

                return false;
            }

            item.Stacksize += NewItem.Stacksize;
            return true;
        }
    }

    public virtual void ClearSlot(InventorySlotHandler slothander)
    {
        item = null;
        if (itemRequirement != null)
            itemRequirement.Reset();
        if (slothander == null) { return; }
        UpdateSlot(slothander);
    }

    public virtual void UpdateSlot(InventorySlotHandler slotHandler)
    {
        if (slotHandler == null) { return; }
        if (item == null || item.Icon == null)
        {
            slotHandler.text.text = "";
            if (OriginalImage != null)
            {
                slotHandler.image.sprite = OriginalImage;
                slotHandler.image.color = Color.white;
                return;
            }
            slotHandler.image.sprite = null;
            slotHandler.image.color = Color.clear;
            return;
        }
        if (item.Stacksize > 1)
        {
            slotHandler.text.text = item.Stacksize.ToString();
        }
        else { slotHandler.text.text = ""; }

        slotHandler.image.sprite = item.Icon;
        slotHandler.image.color = Color.white;
        return;

    }

    public virtual bool CanGrabSlot()
    {
        if (isEmpty) { return false; }
        if (item == null) { return false; }
        return true;
    }

    public virtual int NumberOfItemsCanGrab()
    {
        return item.Stacksize;
    }

    public virtual void GrabItem(InventorySlotHandler HandlerGrabbedFrom, out Item newItemSpot)
    {
        newItemSpot = item;
        ClearSlot(HandlerGrabbedFrom);
    }

}

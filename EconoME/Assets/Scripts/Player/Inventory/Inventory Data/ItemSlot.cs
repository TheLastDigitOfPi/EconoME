using System;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    public event Action OnItemChange;

    /// <summary>
    /// Since the item is being removed from the slot, we don't care what happens to the i
    /// </summary>
    public event Action<Item> OnItemRemoved;
    public event Action<Item> OnItemAdded;

    public int SlotNum;
    [SerializeField] InventoryObject _inventoryGroup;
    [field: SerializeReference] public Item Item { get; private set; }

    public ItemSlot(int slotNum, InventoryObject inventory)
    {
        _inventoryGroup = inventory;
        SlotNum = slotNum;
    }

    public void Initialize(InventoryObject inventory)
    {
        _inventoryGroup = inventory;
        OnItemChange = null;
        if (Item != null)
        {
            Item.ResetEvents();
            Item.OnItemUpdate += UpdateSlot;
        }
        OnItemRemoved = null;
        OnItemAdded = null;
        UpdateSlot();
    }

    public void UpdateSlot()
    {
        OnItemChange?.Invoke();
    }
    public bool HasItem
    {
        get
        {
            if (Item == null || StackSize < 1 || ItemBase == null)
                return false;
            return true;
        }
    }
    public bool IsFull
    {
        get
        {
            if (Item.IndividualItemWeight == -1)
                return true;
            return RemainingStackRoom <= 0;
        }
    }

    public bool TryRemoveItem(out Item removedItem)
    {
        removedItem = null;
        if (Item == null)
            return false;

        removedItem = Item;
        removedItem.OnItemUpdate -= UpdateSlot;
        OnItemRemoved?.Invoke(Item);
        Item = null;
        UpdateSlot();
        return true;
    }

    public bool TrySetItem(Item itemToAdd)
    {
        if (HasItem)
            return false;
        Item = itemToAdd;
        OnItemAdded?.Invoke(Item);
        if (Item != null)
            Item.OnItemUpdate += UpdateSlot;
        UpdateSlot();
        return true;
    }


    public int RemainingStackRoom { get { return MaxItems - Item.Stacksize; } }
    public int MaxItems { get { return MaxInventorySlotWeight / Item.IndividualItemWeight; } }
    int MaxInventorySlotWeight { get { return _inventoryGroup.MaxInventorySlotWeight.Value; } }
    public int StackSize { get { return Item.Stacksize; } set { Item.Stacksize = value; UpdateSlot(); } }
    public ItemBase ItemBase { get { return Item.ItemBase; } }
    public bool SameBase(Item item) { return Item.SameBase(item); }

}
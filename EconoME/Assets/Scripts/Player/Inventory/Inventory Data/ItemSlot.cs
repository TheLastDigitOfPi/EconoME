using System;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    public event Action OnItemChange;
    public int SlotNum;
    [SerializeField] InventoryObject _inventoryGroup;
    [SerializeReference] Item _item;

    public ItemSlot(int slotNum, InventoryObject inventory)
    {
        _inventoryGroup = inventory;
        SlotNum = slotNum;
    }

    public void Initialize(InventoryObject inventory)
    {
        _inventoryGroup = inventory;
        OnItemChange = null;
    }

    public Item Item { set { _item = value; UpdateItem(); } }
    public void UpdateItem()
    {
        OnItemChange?.Invoke();
    }
    public bool HasItem
    {
        get
        {
            if (_item == null || StackSize < 1 || ItemBase == null)
                return false;
            return true;
        }
    }
    public bool IsFull
    {
        get
        {
            if (_item.IndividualItemWeight == -1)
                return true;
            return RemainingStackRoom <= 0;
        }
    }
    public int RemainingStackRoom { get { return MaxItems - _item.Stacksize; } }
    public int MaxItems { get { return MaxInventorySlotWeight / _item.IndividualItemWeight; } }
    int MaxInventorySlotWeight { get { return _inventoryGroup.MaxInventorySlotWeight.Value; } }
    public int StackSize { get { return _item.Stacksize; } set { _item.Stacksize = value; UpdateItem(); } }
    public ItemBase ItemBase { get { return _item.ItemBase; } }
    public bool SameBase(Item item) { return _item.SameBase(item); }
    public Item ItemCopy { get { return _item.Duplicate(); } }

}
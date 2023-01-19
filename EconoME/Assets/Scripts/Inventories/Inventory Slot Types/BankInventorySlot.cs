using System;
using UnityEngine;

[Serializable]
public class BankInventorySlot : InventorySlot
{
    public bool Unlocked;
    public int SlotNumber;
    [SerializeField] TMPro.TextMeshProUGUI BankLockedItemCover;

    public BankInventorySlot() : base()
    {
        slotType = SlotType.Bank;
    }

    public override bool SetSlot(Item NewItem, InventorySlotHandler slotToUpdate)
    {

        bool Success = SearchAllBank(NewItem);
        ResourceBankHandler.Instance.RefineSearch();
        return Success;
        bool SearchAllBank(Item NewItem)
        {
            ResourceBankHandler.Instance.TempResetSearch();
            InventorySlotHandler[] allSlots = ResourceBankHandler.Instance.AllBankSlots;
            for (int i = 0; i < allSlots.Length; i++)
            {
                var handler = allSlots[i];
                var BankSlot = handler.slotData as BankInventorySlot;
                if (!handler.slotData.itemRequirement.isValidItem(NewItem)) { continue; }
                if (BankSlot.item == null || BankSlot.item.ItemName == "") { continue; }

                if (BankSlot.item.Stacksize < 1 && !BankSlot.Unlocked)
                {
                    unlockSlot(handler);
                }
                BankSlot.item.Stacksize += NewItem.Stacksize;
                base.UpdateSlot(handler);
                return true;
                
            }
            return false;
        }
    }

    void unlockSlot(InventorySlotHandler slotToUpdate)
    {
        Unlocked = true;
        if (slotToUpdate == null) { return; }
        item.Stacksize = 0;
        slotToUpdate.image.color = Color.white;
        slotToUpdate.text.text = item.Stacksize.ToString();
        BankLockedItemCover.text = "";
        
    }

    public override int NumberOfItemsCanGrab()
    {
        if (item.CurrentWeight > MaxInventorySlotWeight)
        {
            return MaxInventorySlotWeight * item.Weight;
        }
        return item.Stacksize;
    }

    public override void GrabItem(InventorySlotHandler HandlerGrabbedFrom, out Item newItemSpot)
    {
        Item NewItem = item.Duplicate();
        NewItem.Stacksize = NumberOfItemsCanGrab();
        item.Stacksize -= NewItem.Stacksize;
        base.UpdateSlot(HandlerGrabbedFrom);
        newItemSpot = NewItem;
    }

    public override bool CanGrabSlot()
    {
        if(item == null){return false;}
        return item.Stacksize > 0;
    }

    public override void ClearSlot(InventorySlotHandler slothander)
    {
        item.Stacksize = 0;
        base.UpdateSlot(slothander);
    }

}

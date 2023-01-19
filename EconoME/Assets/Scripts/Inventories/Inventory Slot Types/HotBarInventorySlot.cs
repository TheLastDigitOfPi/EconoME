using System;

[Serializable]
public class HotBarInventorySlot : InventorySlot
{

    public InventorySlotHandler EquivalantHotBarSlot;
    public HotBarInventorySlot() : base()
    {
        slotType = SlotType.Hotbar;
    }

    public override void UpdateSlot(InventorySlotHandler slotHandler)
    {
        base.UpdateSlot(slotHandler);
        (EquivalantHotBarSlot.slotData as HotBarInventorySlot).UpdateMySlot(EquivalantHotBarSlot);
    }

    public void UpdateMySlot(InventorySlotHandler myHandler)
    {
        base.UpdateSlot(myHandler);
    }

}

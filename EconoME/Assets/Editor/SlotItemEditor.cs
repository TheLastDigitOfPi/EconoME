using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InventorySlotHandler)), CanEditMultipleObjects]
public class SlotItemEditor : Editor
{
    InventorySlotTypes SlotOptions;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        InventorySlotHandler handler = (InventorySlotHandler)target;

        if(GUILayout.Button("Force Scriptable Item data to Slot"))
        {
            if(handler.ForceSlotItem == null) {return;}
            handler.slotData.item = handler.ForceSlotItem.CreateItem();
            handler.UpdateSlot();
        }

        int prevSlot = (int)SlotOptions;
        SlotOptions =(InventorySlotTypes)EditorGUILayout.EnumPopup("Select Slot Type", SlotOptions);

        if ((int)SlotOptions != prevSlot)
        {
            switch (SlotOptions)
            {
                case InventorySlotTypes.InventorySlot:
                    handler.slotData = new InventorySlot();
                    break;
                case InventorySlotTypes.HotBarSlot:
                    handler.slotData = new HotBarInventorySlot();
                    break;
                case InventorySlotTypes.BankSlot:
                    handler.slotData =  new BankInventorySlot();
                    break;
                case InventorySlotTypes.ShopSlot:
                    handler.slotData = new ShopInventorySlot();
                    break;
                default:
                    break;
            }
        }
        
    }

}

public enum InventorySlotTypes
{
    InventorySlot,
    HotBarSlot,
    BankSlot,
    ShopSlot
}

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

        if (GUILayout.Button("Force Scriptable Item data to Slot"))
        {
            if (handler.ForceSlotItem == null) { return; }
            handler.ItemSlot.Item = handler.ForceSlotItem.CreateItem();
            handler.UpdateSlot();
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

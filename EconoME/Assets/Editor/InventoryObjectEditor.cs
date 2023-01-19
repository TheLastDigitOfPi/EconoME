using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryObject))]
public class InventoryObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var handler = (InventoryObject)target;

        if (GUILayout.Button("Update Inventory"))
        {
            for (int i = 0; i < handler.data.items.Length; i++)
            {
                if (handler.data.items[i] == null) continue;

                if (handler.data.items[i].ItemBase == null)
                {
                    handler.data.items[i].Stacksize = 0;
                    continue;
                }
                handler.data.items[i] = handler.data.items[i].ItemBase.CreateItem(handler.data.items[i].Stacksize <= 0 ? 1 : handler.data.items[i].Stacksize);
            }
        }
    }

}

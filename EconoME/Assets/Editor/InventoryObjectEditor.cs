using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryObject))]
public class InventoryObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var handler = (InventoryObject)target;

        if (GUILayout.Button("Add Test Item"))
        {
            handler.InsertItemToSlot();
        }
    }

}


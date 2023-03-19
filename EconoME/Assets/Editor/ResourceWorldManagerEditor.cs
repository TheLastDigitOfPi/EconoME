using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResourceWorldManager))]
public class ResourceWorldManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var manager = (ResourceWorldManager)target;
        if (GUILayout.Button("Generate random 3x3 tiles starting at 0, 0"))
        {
            manager.EditorTest3x3();
        }

    }

}
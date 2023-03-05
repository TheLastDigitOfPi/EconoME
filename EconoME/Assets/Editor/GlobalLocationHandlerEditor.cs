using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GlobalLocationHandler), true), CanEditMultipleObjects]
public class GlobalLocationHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GlobalLocationHandler handler = (GlobalLocationHandler)target;
        if (GUILayout.Button("Test Distance Finder"))
        {
            handler.TestDistanceFinder();
        }
    }
}

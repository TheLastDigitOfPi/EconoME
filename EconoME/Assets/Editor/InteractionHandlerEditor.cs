using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractionHandler), true), CanEditMultipleObjects]
public class InteractionHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        InteractionHandler _handler = (InteractionHandler)target;
        if (GUILayout.Button("Test Interaction"))
        {
            _handler.TestTheInteraction();
        }
    }
}

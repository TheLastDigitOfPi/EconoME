using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractionSO), true), CanEditMultipleObjects]
public class InteractionObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        InteractionSO _interaction = (InteractionSO)target;
        if (GUILayout.Button("Refresh GUID"))
        {
            _interaction.resetGUID();
        }
    }
}

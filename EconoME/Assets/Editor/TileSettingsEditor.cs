using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileNodesSetting))]
public class TileSettingsEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var handler = (TileNodesSetting)target;

        if (GUILayout.Button("Add to Dictionary"))
        {
            if (handler.tileGenerationSettings.NodesCanGenerate.ContainsKey(handler.tileGenerationSettings.AddKeyToDictionary))
            {
                return;
            }
            handler.tileGenerationSettings.NodesCanGenerate.Add(handler.tileGenerationSettings.AddKeyToDictionary, handler.tileGenerationSettings.AddValueToDictionary);

        }



    }

}

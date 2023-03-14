using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileGenerator))]
public class TileGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var generator = (TileGenerator)target;
        if (GUILayout.Button("Generate Tile"))
        {
            generator.GenerateTile();
        }

        if (GUILayout.Button("Generate Random Tile"))
        {
            generator.GenerateRandomTile();
        }

        if (GUILayout.Button("Clear Tile"))
        {
            generator.ClearTile();
        }


    }

}
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldTileManager))]
public class WorldTileManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var manager = (WorldTileManager)target;
        if (GUILayout.Button("Generate random 3x3 tiles starting at 0, 0"))
        {
            manager.EditorTest3x3();
        }

        if (GUILayout.Button("Reset"))
        {
            manager.EditorTestReset();
        }

        if (GUILayout.Button("Add Tile to Position"))
        {
            manager.AddTileAtPos();
        }
        if (GUILayout.Button("Remove Tile at Position"))
        {
            manager.TryRemoveTileTest();
        }
    }

}
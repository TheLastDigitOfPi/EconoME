using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomGrassGenerator))]
public class RandomGrassGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var generator = (RandomGrassGenerator)target;
        if (GUILayout.Button("Generate Grass"))
        {
            generator.Generate();
        }

        if (GUILayout.Button("Reset Grass"))
        {
            generator.ResetGrass();
        }


    }

}
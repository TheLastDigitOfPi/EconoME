using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PerlinNoiseGenerator))]
public class PerlinNoiseGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var generator = (PerlinNoiseGenerator)target;
        if (GUILayout.Button("Generate Noise"))
        {
            generator.GenerateNoise();
        }

        if (GUILayout.Button("Generate Random Noise"))
        {
            generator.seed = Random.Range(0, 50000).ToString();
            generator.GenerateNoise();
        }


    }

}

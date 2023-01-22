using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshToMesh : MonoBehaviour
{
    [SerializeField] MeshFilter meshFilter;
    [ContextMenu("GenerateMesh")]
    public void GenerateMesh()
    {
        string path = Application.dataPath + "/Main";
        Debug.Log("Path was " + path);
        
        Mesh mesh = new Mesh();
        var newMesh = Object.Instantiate(mesh) as Mesh;

        var triangles = NavMesh.CalculateTriangulation();
        newMesh.vertices = triangles.vertices;
        newMesh.triangles = triangles.indices;
        AssetDatabase.CreateAsset(meshFilter.sharedMesh, path);
        AssetDatabase.SaveAssets();
        
    }
}
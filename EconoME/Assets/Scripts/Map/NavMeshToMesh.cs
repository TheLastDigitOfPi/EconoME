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
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/Main/", "NewMesh", "asset");
        Debug.Log("Path was " + path);

        path = FileUtil.GetProjectRelativePath(path);
        Mesh mesh = new Mesh();
        var newMesh = Object.Instantiate(mesh) as Mesh;

        var triangles = NavMesh.CalculateTriangulation();
        newMesh.vertices = triangles.vertices;
        newMesh.triangles = triangles.indices;
        MeshUtility.Optimize(newMesh);
        AssetDatabase.CreateAsset(newMesh, path);
        AssetDatabase.SaveAssets();


    }

    [SerializeField] Mesh meshToSplit;
    [ContextMenu("Split Mesh")]
    public void SplitMesh()
    {

        var bounds = meshToSplit.bounds;

        float startX = bounds.min.x;
        float startY = bounds.min.y;
        float chunkSizeX = bounds.max.x / 3;
        float chunkSizeY = bounds.max.y / 3;

        var verticies = meshToSplit.vertices;
        var triangles = meshToSplit.triangles;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Mesh tempMesh = new Mesh();
                Mesh newMesh = Object.Instantiate(tempMesh) as Mesh;


                //Get current chunk data
                //Ex: Chunk 1 is going to be top left, so bounds min (startX to chunkSize
                List<Vector3> newVerticies = new();
                for (int k = 0; k < verticies.Length; k++)
                {
                    //if the verticie is within the chunk, add it to the new mesh
                    if (VerticieWithinBounds(k))
                    {
                        newVerticies.Add(verticies[k]);
                    }
                }
                List<int> newTriangles = new();
                for (int l = 0; l < triangles.Length - 2; l += 3)
                {
                    if (VerticieWithinBounds(triangles[l]) && VerticieWithinBounds(triangles[l + 1]) && VerticieWithinBounds(triangles[l + 2]))
                    {
                        newTriangles.Add(triangles[l]);
                        newTriangles.Add(triangles[l + 1]);
                        newTriangles.Add(triangles[l + 2]);
                    }
                }

                newMesh.vertices = newVerticies.ToArray();
                newMesh.triangles = newTriangles.ToArray();
                SaveMesh(newMesh, "Mesh " + i + " " + j);

                bool VerticieWithinBounds(int pos)
                {
                    return verticies[pos].x.isBetweenInclusive(startX + (i*chunkSizeX), startX + ((i+1) * chunkSizeX)) && verticies[pos].y.isBetweenInclusive(startY + (j*chunkSizeY), startY + ((j+1) * chunkSizeY));
                }

            }
        }

        void SaveMesh(Mesh meshToSave, string name)
        {
            string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/Main/", name, "asset");
            path = FileUtil.GetProjectRelativePath(path);
            Debug.Log("Path was " + path);
            MeshUtility.Optimize(meshToSave);
            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();
        }
    }
}

using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Node Database", menuName = "ScriptableObjects/TileGeneration/Databases/Resource Node Database")]
[System.Serializable]
public class ResourceNodeDatabase : ScriptableObject
{
    [SerializeField] CustomResourceNode[] NodesInDatabase;

    public ResourceNodeHandler FindPrefab(ResourceNodeType Node)
    {
        var FoundNode = NodesInDatabase.FirstOrDefault(n => n.NodeType == Node);

        if (FoundNode == null)
        {
            Debug.LogWarning($"Failed to find Prefab for Node: {Node}");
        }
        return FoundNode.prefab;
    }

    public ResourceNodeHandler FindPrefab(int id)
    {
        var foundNode = NodesInDatabase.FirstOrDefault(p => p.NodeDatabaseID == id);
        if(foundNode == null)
        {
            Debug.LogWarning($"Failed to find Prefab for Node with ID: {id}");
            return null;
        }
        return foundNode.prefab;
    }
}

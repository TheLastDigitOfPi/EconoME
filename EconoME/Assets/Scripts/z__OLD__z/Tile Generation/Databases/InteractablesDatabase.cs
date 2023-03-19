using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Interactables Database", menuName = "ScriptableObjects/TileGeneration/Databases/Interactables Database")]
[System.Serializable]
public class InteractablesDatabase : ScriptableObject
{
    [SerializeField] CustomInteractable[] InteractablesInDatabase;

    public GameObject FindPrefab(int id)
    {
        var foundNode = InteractablesInDatabase.FirstOrDefault(p => p.InteractableID == id);
        if(foundNode == null)
        {
            Debug.LogWarning($"Failed to find Prefab for Node with ID: {id}");
            return null;
        }
        return foundNode.prefab;
    }
}
using UnityEngine;
[CreateAssetMenu(fileName = "New Interactables Object", menuName = "ScriptableObjects/TileGeneration/DataObjects/Custom Interactable")]
public class CustomInteractable : ScriptableObject
{
    public int InteractableID;
    public GameObject prefab;

}

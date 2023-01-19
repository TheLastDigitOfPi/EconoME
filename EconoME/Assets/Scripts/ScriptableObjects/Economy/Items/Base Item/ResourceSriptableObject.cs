using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Item", menuName = "ScriptableObjects/Economy/Items/BaseItems/Resource")]
public class ResourceSriptableObject : ItemScriptableObject
{
    [SerializeField] ResourceType _resourceType;
    [SerializeField] int _tier;
    public int Tier {get {return _tier;}}
    public override ItemType ItemType => _resourceType;
}


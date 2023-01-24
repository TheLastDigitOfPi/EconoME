using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Item", menuName = "ScriptableObjects/Economy/Items/BaseItems/Resource")]
public class ResourceBase : ItemBase
{
    [SerializeField] ResourceType _resourceType;
    [SerializeField] int _tier;
    public int Tier {get {return _tier;}}

    private void OnEnable()
    {
        this.ItemType = ItemType.Resource;
    }

    public override Item CreateItem(int stackSize)
    {
        return new Resource(this, stackSize);
    }
}

public enum ResourceType
{
    Lumber,
    Mineral
}


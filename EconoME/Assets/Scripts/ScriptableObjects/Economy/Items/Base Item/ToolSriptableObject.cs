using UnityEngine;

[CreateAssetMenu(fileName = "New Tool Item", menuName = "ScriptableObjects/Economy/Items/BaseItems/Tool")]
public class ToolSriptableObject : ItemScriptableObject
{
    [SerializeField] int _toolTier;
    [SerializeField] int _damage;
    [SerializeField] TextureGroup[] _toolSwingAnimations = new TextureGroup[4];
    [SerializeField] ToolType _toolType;

    public int ToolTier { get => _toolTier; }
    public int Damage { get => _damage; }
    public TextureGroup[] ToolSwingAnimations {get {return _toolSwingAnimations; } }

    public override ItemType ItemType => _toolType;

    public override Item CreateItem(int stackSize)
    {
        return new Tool(this);
    }
}


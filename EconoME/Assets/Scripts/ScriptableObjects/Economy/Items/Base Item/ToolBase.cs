using UnityEngine;

[CreateAssetMenu(fileName = "New Tool Item", menuName = "ScriptableObjects/Economy/Items/BaseItems/Tool")]
public class ToolBase : ItemBase
{
    [field: SerializeField] public int ToolTier {get; private set;}
    [field: SerializeField] public int Damage {get; private set; }
    [field: SerializeField] public float Speed {get; private set;}

    [SerializeField] TextureGroup[] _toolSwingAnimations = new TextureGroup[4];
    [SerializeField] public ToolType ToolType { get; private set; }

    public TextureGroup[] ToolSwingAnimations { get { return _toolSwingAnimations; } }

    public override Item CreateItem(int stackSize)
    {
        return new Tool(this);
    }
}

public enum ToolType
{
    Axe,
    Sword,
    Pickaxe
}


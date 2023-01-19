using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Item", menuName = "ScriptableObjects/Economy/Items/BaseItems/Tile")]
public class TileSriptableObject : ItemScriptableObject
{
    [SerializeField] TileType tileType;
    public override ItemType ItemType => tileType;
}


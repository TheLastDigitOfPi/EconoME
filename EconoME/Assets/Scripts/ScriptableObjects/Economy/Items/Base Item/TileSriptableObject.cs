using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Item", menuName = "ScriptableObjects/Economy/Items/BaseItems/Tile")]
public class TileSriptableObject : ItemBase
{
    [SerializeField] TileType tileType;
}

public enum TileType
{
    Forest,
    Arctic,
    Desert
}

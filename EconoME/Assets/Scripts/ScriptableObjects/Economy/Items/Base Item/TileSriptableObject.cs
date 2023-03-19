using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Item", menuName = "ScriptableObjects/Economy/Items/BaseItems/Tile")]
public class TileSriptableObject : ItemBase
{
    [SerializeField] TileBiome tileType;
}

public enum TileBiome
{
    Forest,
    Plains,
    DarkForest,
    Arctic,
    Desert,
    Mountain
}

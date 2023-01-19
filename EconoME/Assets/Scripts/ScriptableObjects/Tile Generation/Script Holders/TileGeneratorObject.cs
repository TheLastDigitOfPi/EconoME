using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Generator Object", menuName = "ScriptableObjects/TileGeneration/DataObjects/Tile Generator Object")]
public abstract class TileGeneratorObject : ScriptableObject
{
    public abstract InteractablesDatabase InteractablesDatabase { get; }
    public abstract ResourceNodeDatabase NodeDatabase { get; }
    public abstract TileDatabase TileDatabase { get; }

    public abstract WorldTile GenerateTile(TileType type, int tier);
    public abstract WorldTile GenerateTile(TileGenerationPreset preset, TileGenerationSettings settings = null);
}

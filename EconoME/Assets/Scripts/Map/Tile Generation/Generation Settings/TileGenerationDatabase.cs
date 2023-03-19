using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Generation Database", menuName = "ScriptableObjects/Tiles/Tile Generation Database")]
public class TileGenerationDatabase : ScriptableObject
{
    [SerializeField] List<TileBaseSettings> TileBaseSettings;

    public TileBaseSettings GetTileSettings(TileSettings settings)
    {
        return TileBaseSettings.Find(a => a.BiomeType == settings.TileBiome);

    }

}

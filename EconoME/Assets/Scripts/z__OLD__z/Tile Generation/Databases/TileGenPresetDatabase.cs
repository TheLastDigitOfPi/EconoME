using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Generation Preset Database", menuName = "ScriptableObjects/TileGeneration/Databases/Tile Generation Preset Database")]
public class TileGenPresetDatabase : ScriptableObject
{
    [SerializeField] TileGenerationPreset[] PresetsInDatabase;


    public TileGenerationPreset FindPreset(TileBiome type, int tier)
    {
       return PresetsInDatabase.FirstOrDefault(t => t.Type == type && t.Tier == tier);
    }
}


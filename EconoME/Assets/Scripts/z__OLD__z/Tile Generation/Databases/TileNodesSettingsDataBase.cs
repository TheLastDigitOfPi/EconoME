using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Default Tile Database", menuName = "ScriptableObjects/TileGeneration/Databases/Default Tile Database")]
public class TileNodesSettingsDataBase : ScriptableObject
{
    [SerializeField] TileNodesSetting[] tileGenerationSettings;
    private TileGenerationSettings[] settings;

    private void OnEnable()
    {
        settings = new TileGenerationSettings[tileGenerationSettings.Length];
        for (int i = 0; i < tileGenerationSettings.Length; i++)
        {
            settings[i] = tileGenerationSettings[i].tileGenerationSettings;
        }
    }


    public TileGenerationSettings GetDefaultSetting(TileBiome type, int tier)
    {
        TileGenerationSettings setting = settings.FirstOrDefault(s => s.TileType == type && s.Tier == tier);

        if (setting == null)
        {
            Debug.LogWarning(("Failed to find a Default Tile Generation setting for tier: " + tier));
        }

        return setting;
    }
}
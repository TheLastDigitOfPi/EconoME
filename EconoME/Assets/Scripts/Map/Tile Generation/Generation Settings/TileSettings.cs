using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The settings for a specific tile. These are changed at runtime and what will be saved to disk
/// </summary>
[Serializable]
public class TileSettings
{

    [field: Header("Tile Type Settings")]
    /// <summary>
    /// The level of tile. Higher tiers allow for higher tiered mobs and nodes to be spawned as well as has slighly better spawn rates 
    /// </summary>
    [field: SerializeField] public int Tier { get; private set; }

    /// <summary>
    /// The Biome of the given Tile.
    /// </summary>
    [field: SerializeField] public TileBiome TileBiome { get; private set; }


    [field: Header("Object Spawn Settings")]
    /// <summary>
    /// The list of given nodes that this tile can spawn
    /// </summary>
    [field: SerializeField] public List<ResourceNodeSpawnSetting> NodesThatCanBeGenerated { get; private set; } = new();
    /// <summary>
    /// The list of given mobs that this tile can spawn
    /// </summary>
    [field: SerializeField] public List<MobSpawnSetting> MobsThatCanSpawn { get; private set; } = new();

    [field: Header("Seed Settings")]
    /// <summary>
    /// The Seed used for generating the tile
    /// </summary>
    [field: SerializeField] public string BaseSeed { get; private set; }
    [field: SerializeField] public bool UseRandomSeed { get; set; } = false;

    [field: SerializeField] public float GrassArea { get; private set; } = 0.6f;
    [field: SerializeField] public float GroundArea { get; private set; } = 0.7f;
    [field: SerializeField, Range(0.7f, 1f)] public float WaterArea { get; private set; } = 0.7f;
    public GameObject GetNodePrefab(int seed, int loop)
    {
        System.Random rand = new(seed + loop);
        int loops = 0;
        while (true)
        {
            loops++;
            if (loops > 10000)
            {
                Debug.LogWarning("Reached max loops in trying to get random node!");
                return null;
            }
            var chance = rand.NextDouble();
            var possibleNode = NodesThatCanBeGenerated.RandomListItem(rand);
            if (possibleNode.ChanceOfSpawning >= chance)
                return possibleNode.objectPrefab.NodePrefab;
        }

    }

    public ItemBase ItemBase { get; private set; }

    public TileSettings(ItemBase itemBase, int tier, TileBiome tileBiome, List<ResourceNodeSpawnSetting> nodesThatCanBeGenerated, List<MobSpawnSetting> mobsThatCanSpawn = null, string baseSeed = default, float grassArea = 0.6f, float groundArea = 0.7f, float waterArea = 0.7f)
    {
        Tier = tier;
        TileBiome = tileBiome;
        NodesThatCanBeGenerated = nodesThatCanBeGenerated;
        MobsThatCanSpawn = mobsThatCanSpawn;
        BaseSeed = baseSeed;
        if (baseSeed == default)
            UseRandomSeed = true;
        GrassArea = grassArea;
        GroundArea = groundArea;
        WaterArea = waterArea;
        ItemBase = itemBase;
    }
}


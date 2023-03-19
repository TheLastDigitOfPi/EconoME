using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Represents an object that can spawn on a tile. 
/// Should be able to give the following:
/// The Prefab in order to spawn it
/// The chance that this can be spawned
/// Where it can be spawned
/// </summary>
public class TileObjectSpawnSetting<T> : ScriptableObject
{
    /// <summary>
    /// The prefab used to instantiate the object in game
    /// </summary>
    [field: SerializeField] public T objectPrefab { get; private set; }

    /// <summary>
    /// The chance that this object will be able to spawn given that it is randomly chosen from a list of possible objects to spawn
    /// </summary>
    [field: SerializeField, Range(0, 1)] public float ChanceOfSpawning { get; private set; }

    [SerializeField] SortingType SortingType = SortingType.WhiteList;
    /// <summary>
    /// List of biomes that this object can or cannot be spawned in depending on the sorting type
    /// </summary>

    [SerializeField] List<TileBiome> _biomesCheckList = new();
    /// <summary>
    /// The minimum tier tile that this object can be spwaned in
    /// </summary>
    [SerializeField] int _minimumTier;

}


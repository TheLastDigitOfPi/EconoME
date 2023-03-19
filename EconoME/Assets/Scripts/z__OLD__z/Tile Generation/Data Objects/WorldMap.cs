using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New World Data", menuName = "ScriptableObjects/TileGeneration/DataObjects/WorldMap")]
public class WorldMap : ScriptableObject
{
    public SerializableDictionary<Vector2Int, WorldTile> WorldTiles;

    public WorldMap()
    {
        WorldTiles = new SerializableDictionary<Vector2Int, WorldTile>();
    }
}

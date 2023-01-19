using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldTilesData
{
    public SerializableDictionary<Vector2Int, WorldTile> WorldTiles;
    /*
    I need all Tileobject data here
    Within that data I need
    - Stats
    - Tilemaps
    - Interactables
    - Resource Nodes
    */

    //Initialize data
    public WorldTilesData()
    {
        WorldTiles = new SerializableDictionary<Vector2Int, WorldTile>();
    }
}

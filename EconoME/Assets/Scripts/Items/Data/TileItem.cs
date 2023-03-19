using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Contains the data specific to a tile and its temporary generated data from the seed
/// </summary>
[Serializable]
public class TileItem : Item
{
    const int DIVIDER = 69420;

    /// <summary>
    /// The seed for this specific tile. Used to generate the tile's visuals as well as locations that stuff can spawn on it
    /// </summary>
    public int Seed { get; private set; }

    /// <summary>
    /// The settings of this current tile, updated as the game progresses
    /// </summary>
    public TileSettings TileSettings { get; private set; }


//Objects Currently on Tile
    
    //Resource Nodes currently on tile
    public List<ResourceNode> ResrouceNodesOnTile { get; private set; } = new();

    //Mobs currently on tile

    //Structures currently on tile

    //Data generated from seed
    List<Vector2Int> _possibleNodePlacements;
    
    float[,] _noise;
    bool _initialized = false;
    int _height;
    int _width;

    public void Initialize(int width, int height)
    {
        _width = width;
        _height = height;
        _noise = PerlinNoiseGenerator.GeneratePerlinNoise2D(width / 2, height / 2, Seed, scale: 3.5f);
        ResrouceNodesOnTile = new();
        GetPossibleNodePlacements();

        _initialized = true;
    }

    /// <summary>
    /// Get the node placements for this tile. If the tile data is not initialized, will regenerate the data
    /// </summary>
    /// <returns>List of Coordinates for nodes that can be placed</returns>
    public List<Vector2Int> GetPossibleNodePlacements()
    {
        if (_initialized)
            return _possibleNodePlacements;

        List<Vector2Int> possiblePlacements = new();
        var grassArea = TileSettings.GrassArea;
        for (int x = 1; x < _width/2 - 1; x++)
        {
            for (int y = 1; y < _height/2 - 1; y++)
            {
                //Check if this spot can contain a tree
                if (_noise[x, y] > grassArea)
                    continue;

                //If one of its neighbors is invalid, skip this spot
                if (_noise[x + 1, y] > grassArea)
                    continue;
                if (_noise[x - 1, y] > grassArea)
                    continue;
                if (_noise[x, y - 1] > grassArea)
                    continue;
                if (_noise[x, y + 1] > grassArea)
                    continue;
                if (_noise[x + 1, y + 1] > grassArea)
                    continue;
                if (_noise[x - 1, y + 1] > grassArea)
                    continue;
                if (_noise[x - 1, y - 1] > grassArea)
                    continue;
                if (_noise[x + 1, y - 1] > grassArea)
                    continue;

                possiblePlacements.Add(new Vector2Int(x, y));
            }
        }
        _possibleNodePlacements = possiblePlacements;
        return possiblePlacements;

    }

    public float[,] TryGetNoise()
    {
        return _noise;
    }

    public TileItem(TileItem other) : base(other)
    {
    }

    public TileItem(TileSettings settings) : base(settings.ItemBase)
    {
        TileSettings = settings;

        if (settings.UseRandomSeed)
        {
            System.Random rand = new();
            Seed = rand.Next(0, 69420).GetHashCode();
            return;
        }
        Seed = settings.BaseSeed.GetHashCode();
    }




    public void OnGameTick()
    {

    }

    void GenerateResourceNodes()
    {

    }


}



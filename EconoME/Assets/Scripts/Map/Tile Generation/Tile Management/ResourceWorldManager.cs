using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Manages actions and data exclusive to the resource world and the tiles it holds
/// </summary>
public class ResourceWorldManager : MonoBehaviour
{
    //Static data
    public static ResourceWorldManager Instance;

    //Events
    public event Action OnMapUpdate;

    [field: SerializeField] public TileGenerationDatabase database { get; private set; }


    //Public Data



    //Local data
    [SerializeField] WorldTileHandler prefab;

    /// <summary>
    /// The tiles currently placed in the resource world with a key of their space on the grid
    /// </summary>
    Dictionary<Vector2Int, TileItem> _placedTiles = new();

    const int TILE_SIZE = 32;
    const int DISTANCE_BETWEEN_TILES = 3;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 resource world manager found!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        WorldTimeManager.OnGameTick += UpdateTiles;
    }

    /// <summary>
    /// Called every game tick to update when tiles should do an action
    /// </summary>
    private void UpdateTiles()
    {
        foreach (var tile in _placedTiles.Values)
        {
            tile.OnGameTick();
        }
    }

    public GridNeighbors<TileItem> GetTileFriends(Vector2Int center)
    {
        return new GridNeighbors<TileItem>(_placedTiles, center);
    }

    public bool TryAddTile(Vector2Int pos, TileItem tile)
    {
        return _placedTiles.TryAdd(pos, tile);
    }

    public bool TryRemoveTile(Vector2Int pos, out TileItem tileRemoved)
    {
        tileRemoved = null;
        //Check if there is a tile in the desired location
        if (!_placedTiles.ContainsKey(pos) || _placedTiles[pos] == null)
            return false;

        tileRemoved = _placedTiles[pos];
        return true;
    }

    internal void ResetTiles()
    {
        _placedTiles.Clear();
    }
}

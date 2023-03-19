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

    /// <summary>
    /// Attempt to place a given tile at the desired location
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>Returns true if able to place tile</returns>
    public bool TryPlaceTile(TileItem tile, Vector2Int pos)
    {
        //Check that there is no tile there
        if (_placedTiles.ContainsKey(pos))
            return false;

        //Place tile

        var manager = WorldTileManager.Instance;

        _placedTiles.Add(pos, tile);
        var tileHandler = Instantiate(prefab, manager.transform);
        tileHandler.Initialize(tile);
        tileHandler.transform.position = TilePositionToWorldPosition(pos);
        tileHandler.LoadTileCorroutine();
        return true;
    }

    Vector3 TilePositionToWorldPosition(Vector2Int tilePos)
    {
        var x = tilePos.x * TILE_SIZE + tilePos.x * DISTANCE_BETWEEN_TILES;
        var y = tilePos.y * TILE_SIZE + tilePos.y * DISTANCE_BETWEEN_TILES;
        return new Vector3(x, y, 0);
    }

    public bool TryRemoveTile(Vector2Int pos, out TileItem tileRemoved)
    {
        tileRemoved = null;
        //Check if there is a tile in the desired location
        if (!_placedTiles.ContainsKey(pos) || _placedTiles[pos] == null)
            return false;

        return true;
    }

    [Header("Testing")]
    [SerializeField] List<ResourceNodeSpawnSetting> nodeSettings = new();

    public void EditorTest3x3()
    {

#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            Debug.LogWarning("Can only test while game is running");
            return;
        }
#endif

        List<Vector2Int> testPositions = new();
        Vector2Int pos1 = Vector2Int.zero;
        testPositions.Add(pos1);
        testPositions.Add(pos1 + Vector2Int.right);
        testPositions.Add(pos1 + Vector2Int.left);
        testPositions.Add(pos1 + Vector2Int.up);
        testPositions.Add(pos1 + Vector2Int.down);
        testPositions.Add(pos1 + Vector2Int.right + Vector2Int.down);
        testPositions.Add(pos1 + Vector2Int.right + Vector2Int.up);
        testPositions.Add(pos1 + Vector2Int.left + Vector2Int.down);
        testPositions.Add(pos1 + Vector2Int.left + Vector2Int.up);



        List<TileBiome> biomes = new();
        biomes.Add(TileBiome.Forest);
        biomes.Add(TileBiome.Arctic);
        biomes.Add(TileBiome.DarkForest);
        biomes.Add(TileBiome.Desert);
        foreach (var pos in testPositions)
        {
            var biome = biomes.RandomListItem();
            TileSettings settings = new(null, 0, biome, nodesThatCanBeGenerated: nodeSettings, baseSeed: UnityEngine.Random.Range(0, 7845124).ToString());
            TileItem newTile = new(settings);
            TryPlaceTile(newTile, pos);
        }

    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTileManager : MonoBehaviour
{
    const int TILE_SIZE_X = 32;
    const int TILE_SIZE_Y = 34;

    const float BRIDGE_OFFSET_X = 1.5f;
    const float BRIDGE_OFFSET_Y = 1f;
    const int DISTANCE_BETWEEN_TILES = 3;
    const float PILAR_OFFSET = 1.5f;
    const float SIDE_OPENING_SIZE = 1f;

    public static WorldTileManager Instance;

    //Events
    public event Action OnMapUpdate;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 WorldTileManager Found");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    Dictionary<Vector2Int, WorldTileHandler> _placedTiles = new();

    //Local data
    [SerializeField] WorldTileHandler prefab;
    [SerializeField] Transform horizontalBridgePrefab;
    [SerializeField] Transform verticalBridgePrefab;
    [SerializeField] Transform pillarPrefab;
    [SerializeField] Transform bridgesParent;
    [SerializeField] Transform pilarsParent;
    [SerializeField] ItemBase TileBase;
    [field: SerializeField] public TileGenerationDatabase database { get; private set; }

    [Header("Testing")]
    [SerializeField] List<ResourceNodeSpawnSetting> nodeSettings = new();
    [SerializeField] bool UseFasterLoading = false;
    [SerializeField] bool HideLoading = false;
    [SerializeField] bool ShowLogs = true;
    [SerializeField] bool Generate3x3OnStart = true;
    /// <summary>
    /// Attempt to place a given tile at the desired location
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>Returns true if able to place tile</returns>
    public bool TryPlaceTile(TileItem tile, Vector2Int pos, out WorldTileHandler tileHandler)
    {
        tileHandler = null;
        var manager = ResourceWorldManager.Instance;
        //Check that there is no tile there
        if (!manager.TryAddTile(pos, tile))
            return false;

        //Place tile
        tileHandler = Instantiate(prefab, transform);
        tileHandler.Initialize(tile, pos);
        _placedTiles.Add(pos, tileHandler);
        tileHandler.transform.position = ((Vector3)WorldPosOfTile(pos));
        TileLoaded(tileHandler);
        tileHandler.LoadTileCorroutine(UseFasterLoading, HideLoading, ShowLogs: ShowLogs);
        return true;
    }
    private void Start()
    {

#if UNITY_EDITOR
        if (Generate3x3OnStart)
            EditorTest3x3();
#endif
    }
    private void TileLoaded(WorldTileHandler tileHandler)
    {
        var manager = ResourceWorldManager.Instance;
        //I would call it tile neighbors but I am afraid neighbors is spelled wrong and I refuse to look it up
        var tileFriends = manager.GetTileFriends(tileHandler.TilePos);

        //Once the tile is loaded, we need to check if it has any neighbors.
        //If it does, we build a bridge between them
        //If it doesn't, we add a tile totem to that side

        CheckSide(GridNeighborPos.Left);
        CheckSide(GridNeighborPos.Right);
        CheckSide(GridNeighborPos.Top);
        CheckSide(GridNeighborPos.Bottom);

        void CheckSide(GridNeighborPos side)
        {
            if (tileFriends.TryGetPos(side, out _))
            {
                AddBridge(tileHandler.TilePos, side.GridToDirection());
                return;
            }
            AddPilar(tileHandler.TilePos, side.GridToDirection());
        }

        void AddBridge(Vector2Int tilePos, Direction otherPos)
        {
            Transform bridge;
            if (otherPos == Direction.Right || otherPos == Direction.Left)
                bridge = Instantiate(horizontalBridgePrefab, bridgesParent);
            else
                bridge = Instantiate(verticalBridgePrefab, bridgesParent);

            bridge.position = GetBridgePos(tilePos, otherPos);
            var handler = bridge.GetComponent<BridgeHandler>();
            tileHandler.OnTileLoad += handler.DropIn;
            if (_placedTiles.TryGetValue(tilePos, out var tile1))
            {
                tile1.OpenSide(otherPos);
                tile1.TileConnectors.AddConnector(bridge, otherPos);
            }

            if (_placedTiles.TryGetValue(tilePos.GetNeighborPos(otherPos), out var tile2))
            {
                tile2.TileConnectors.AddConnector(bridge, otherPos.Inverse());
                tile2.OpenSide(otherPos.Inverse());
            }
        }

        void AddPilar(Vector2Int tilePos, Direction otherPos)
        {
            var pillar = Instantiate(pillarPrefab, pilarsParent);
            pillar.position = GetPilarPos(tilePos, otherPos);

            var pillarHandler = pillar.GetComponent<PillarHandler>();
            pillarHandler.Initialize(tilePos, tilePos.GetNeighborPos(otherPos));

            if (_placedTiles.TryGetValue(tilePos, out var tile1))
            {
                tile1.TileConnectors.AddConnector(pillar, otherPos);
                tile1.CloseSide(otherPos);
            }

            if (_placedTiles.TryGetValue(tilePos.GetNeighborPos(otherPos), out var tile2))
            {
                tile2.TileConnectors.AddConnector(pillar, otherPos.Inverse());
                tile2.CloseSide(otherPos.Inverse());
            }
        }

    }
    Vector3 GetBridgePos(Vector2Int tilePos, Direction otherPos)
    {

        switch (otherPos)
        {
            case Direction.Left:
                return new Vector3(WorldPosOfTile(tilePos).x - BRIDGE_OFFSET_X, WorldPosOfTile(tilePos).y + TILE_SIZE_Y / 2);
            case Direction.Right:
                return new Vector3(WorldPosOfTile(tilePos).x + TILE_SIZE_X + BRIDGE_OFFSET_X, WorldPosOfTile(tilePos).y + TILE_SIZE_Y / 2);
            case Direction.Up:
                return new Vector3(WorldPosOfTile(tilePos).x + TILE_SIZE_X / 2, WorldPosOfTile(tilePos).y + TILE_SIZE_Y + DISTANCE_BETWEEN_TILES / 2f + BRIDGE_OFFSET_Y);
            case Direction.Down:
                return new Vector3(WorldPosOfTile(tilePos).x + TILE_SIZE_X / 2, WorldPosOfTile(tilePos).y - DISTANCE_BETWEEN_TILES / 2f + BRIDGE_OFFSET_Y);
            default:
                return Vector3.zero;
        }
    }
    Vector3 GetPilarPos(Vector2Int tilePos, Direction otherPos)
    {

        switch (otherPos)
        {
            case Direction.Left:
                return new Vector3(WorldPosOfTile(tilePos).x + PILAR_OFFSET, WorldPosOfTile(tilePos).y + TILE_SIZE_Y / 2);
            case Direction.Right:
                return new Vector3(WorldPosOfTile(tilePos).x + TILE_SIZE_X - PILAR_OFFSET, WorldPosOfTile(tilePos).y + TILE_SIZE_Y / 2);
            case Direction.Up:
                return new Vector3(WorldPosOfTile(tilePos).x + TILE_SIZE_X / 2, WorldPosOfTile(tilePos).y + TILE_SIZE_Y - PILAR_OFFSET);
            case Direction.Down:
                return new Vector3(WorldPosOfTile(tilePos).x + TILE_SIZE_X / 2, WorldPosOfTile(tilePos).y + PILAR_OFFSET + TILE_SIZE_Y - TILE_SIZE_X);
            default:
                return Vector3.zero;
        }
    }

    public GridNeighbors<WorldTileHandler> GetTileFriends(Vector2Int center)
    {
        return new GridNeighbors<WorldTileHandler>(_placedTiles, center);
    }
    void RemoveConnections(WorldTileHandler handler)
    {
        //I would call it tile neighbors but I am afraid neighbors is spelled wrong and I refuse to look it up
        var tileFriends = GetTileFriends(handler.TilePos);
        CheckSide(GridNeighborPos.Left);
        CheckSide(GridNeighborPos.Right);
        CheckSide(GridNeighborPos.Top);
        CheckSide(GridNeighborPos.Bottom);

        void CheckSide(GridNeighborPos side)
        {
            //Close the side if there is a tile there
            if (tileFriends.TryGetPos(side, out var otherTile))
            {
                //Close the side
                otherTile.CloseSide(side.GridToDirection().Inverse());

                //Remove bridge 
                otherTile.TileConnectors.RemoveConnector(side.GridToDirection().Inverse());

                //Add pillar
                var newPillar = Instantiate(pillarPrefab, pilarsParent);
                newPillar.position = GetPilarPos(otherTile.TilePos, side.GridToDirection().Inverse());
                var pillarHandler = newPillar.GetComponent<PillarHandler>();
                pillarHandler.Initialize(otherTile.TilePos, handler.TilePos);
                otherTile.TileConnectors.AddConnector(newPillar, side.GridToDirection().Inverse());
                return;
            }
            //Otherwise remove the pillar that was there
            handler.TileConnectors.RemoveConnector(side.GridToDirection());
        }
    }
    Vector2 WorldPosOfTile(Vector2Int tilePos)
    {
        var x = tilePos.x * TILE_SIZE_X + tilePos.x * DISTANCE_BETWEEN_TILES;
        var y = tilePos.y * TILE_SIZE_Y + tilePos.y * DISTANCE_BETWEEN_TILES;
        return new Vector3(x, y, 0);
    }

    public bool TryGetTileData(Vector2Int tilePos, out TileItem tileData)
    {
        tileData = null;
        if (_placedTiles.TryGetValue(tilePos, out var tile))
        {
            tileData = tile.TileData;
        }
        return tileData != null;
    }
    public bool TryRemoveTile(WorldTileHandler tileToRemove)
    {
        if (!ResourceWorldManager.Instance.TryRemoveTile(tileToRemove.TilePos, out var removedTile))
            return false;

        _placedTiles.Remove(tileToRemove.TilePos);
        RemoveConnections(tileToRemove);
        Destroy(tileToRemove.gameObject);
        GroundItemManager.Instance.SpawnItem(removedTile, PlayerMovementController.Instance.PlayerPosition.Value, out _);
        return true;

    }

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
            TileSettings settings = new(TileBase, 0, biome, nodesThatCanBeGenerated: nodeSettings, baseSeed: UnityEngine.Random.Range(0, 7845124).ToString());
            TileItem newTile = new(settings);
            TryPlaceTile(newTile, pos, out _);
        }

    }

    public void TryRemoveTileTest()
    {

#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            Debug.LogWarning("Can only test while game is running");
            return;
        }
#endif
        TryRemoveTile(_placedTiles[AddTilePos]);
    }
    public void EditorTestReset()
    {

#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            Debug.LogWarning("Can only test while game is running");
            return;
        }
#endif
        _placedTiles.Clear();
        ResourceWorldManager.Instance.ResetTiles();
        transform.KillChildren();
        bridgesParent = Instantiate(new GameObject() { name = "Bridges" }, transform).transform;
        pilarsParent = Instantiate(new GameObject() { name = "Pillars" }, transform).transform;


    }

    [SerializeField] Vector2Int AddTilePos;
    public void AddTileAtPos()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            Debug.LogWarning("Can only test while game is running");
            return;
        }
#endif
        List<TileBiome> biomes = new();
        biomes.Add(TileBiome.Forest);
        biomes.Add(TileBiome.Arctic);
        biomes.Add(TileBiome.DarkForest);
        biomes.Add(TileBiome.Desert);

        var biome = biomes.RandomListItem();
        TileSettings settings = new(TileBase, 0, biome, nodesThatCanBeGenerated: nodeSettings, baseSeed: UnityEngine.Random.Range(0, 7845124).ToString());
        TileItem newTile = new(settings);
        TryPlaceTile(newTile, AddTilePos, out _);

    }
}



public class GridNeighbors<T>
{
    T _topLeft;
    T _top;
    T _topRight;
    T _left;
    T _right;
    T _bottomLeft;
    T _bottomRight;
    T _bottom;

    public bool TryGetPos(GridNeighborPos pos, out T data)
    {
        data = default;
        switch (pos)
        {
            case GridNeighborPos.TopLeft:
                if (_topLeft == null)
                {
                    return false;
                }
                data = _topLeft;
                return true;
            case GridNeighborPos.Top:
                if (_top == null)
                {
                    return false;
                }
                data = _top;
                return true;
            case GridNeighborPos.TopRight:
                if (_topRight == null)
                {
                    return false;
                }
                data = _topRight;
                return true;
            case GridNeighborPos.Left:
                if (_left == null)
                {
                    return false;
                }
                data = _left;
                return true;
            case GridNeighborPos.Right:
                if (_right == null)
                {
                    return false;
                }
                data = _right;
                return true;
            case GridNeighborPos.BottomLeft:
                if (_bottomLeft == null)
                {
                    return false;
                }
                data = _bottomLeft;
                return true;
            case GridNeighborPos.BottomRight:
                if (_bottomRight == null)
                {
                    return false;
                }
                data = _bottomRight;
                return true;
            case GridNeighborPos.Bottom:
                if (_bottom == null)
                {
                    return false;
                }
                data = _bottom;
                return true;
            default:
                return false;
        }
    }

    public GridNeighbors(Dictionary<Vector2Int, T> data, Vector2Int center)
    {
        data.TryGetValue(center + Vector2Int.left + Vector2Int.up, out _topLeft);
        data.TryGetValue(center + Vector2Int.right + Vector2Int.up, out _topRight);
        data.TryGetValue(center + Vector2Int.up, out _top);
        data.TryGetValue(center + Vector2Int.down, out _bottom);
        data.TryGetValue(center + Vector2Int.left + Vector2Int.down, out _bottomLeft);
        data.TryGetValue(center + Vector2Int.right + Vector2Int.down, out _bottomRight);
        data.TryGetValue(center + Vector2Int.left, out _left);
        data.TryGetValue(center + Vector2Int.right, out _right);

    }
}

public enum GridNeighborPos
{
    TopLeft,
    Top,
    TopRight,
    Left,
    Right,
    BottomLeft,
    BottomRight,
    Bottom
}
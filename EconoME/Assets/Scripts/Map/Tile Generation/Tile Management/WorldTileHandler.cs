using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Components;
using System;

public class WorldTileHandler : MonoBehaviour
{
    [SerializeField] Tilemap IslandTiles;
    [SerializeField] Tilemap GrassTiles;
    [SerializeField] Tilemap DetailsTiles;
    [SerializeField] Tilemap WaterTiles;

    [SerializeField] Transform NodesParent;

    [SerializeField] EdgeCollider2D[] Colliders;

    public TileConnectors TileConnectors { get; private set; } = new();
    const int HEIGHT = 32;
    const int WIDTH = 32;
    TileItem _tileData;
    public Vector2Int TilePos { get; private set; }

    //Helpers
    TileGenerationDatabase database { get { return ResourceWorldManager.Instance.database; } }

    public event Action<WorldTileHandler> OnTileLoad;
    public void Initialize(TileItem data, Vector2Int pos)
    {
        _tileData = data;
        TilePos = pos;
    }


    /// <summary>
    /// Loads tile into world
    /// </summary>
    /// <returns>Returns true if succesfully loads tile</returns>
    public bool LoadTile()
    {
        if (_tileData == null)
            return false;

        _tileData.Initialize(WIDTH, HEIGHT);

        var Seed = _tileData.Seed;
        var tileSettings = database.GetTileSettings(_tileData.TileSettings);

        //Tiles
        var Water = tileSettings.Water;
        var Grass = tileSettings.Grass;
        var CliffRT = tileSettings.Cliffs;

        //Noise
        var noise = _tileData.TryGetNoise();

        LoadBase();
        LoadGrass();
        LoadWater();
        LoadNodes();

        void LoadBase()
        {
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT + 2; y++)
                {
                    IslandTiles.SetTile(new Vector3Int(x, y, 0), CliffRT);
                }
            }
        }
        void LoadGrass()
        {
            if (Grass == null)
                return;

            var _grassArea = _tileData.TileSettings.GrassArea;

            for (int x = 0; x < noise.GetLength(0); x++)
            {
                for (int y = 0; y < noise.GetLength(1); y++)
                {
                    if (noise[x, y] < _grassArea)
                    {
                        GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1, 0), Grass);
                        GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1, 0), Grass);
                        GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1 - 1, 0), Grass);
                        GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1 - 1, 0), Grass);

                        try
                        {
                            if (noise[x + 1, y] > _grassArea)
                                continue;
                            if (noise[x - 1, y] > _grassArea)
                                continue;
                            if (noise[x, y - 1] > _grassArea)
                                continue;
                            if (noise[x, y + 1] > _grassArea)
                                continue;
                            if (noise[x + 1, y + 1] > _grassArea)
                                continue;
                            if (noise[x - 1, y + 1] > _grassArea)
                                continue;
                            if (noise[x - 1, y - 1] > _grassArea)
                                continue;
                            if (noise[x + 1, y - 1] > _grassArea)
                                continue;
                        }
                        catch (System.Exception)
                        {
                        }


                    }
                }
            }
        }
        void LoadWater()
        {
            if (Water == null)
                return;

            var _waterArea = _tileData.TileSettings.WaterArea;
            for (int x = 1; x < noise.GetLength(0) - 1; x++)
            {
                for (int y = 1; y < noise.GetLength(1) - 1; y++)
                {
                    if (noise[x, y] > _waterArea)
                    {
                        WaterTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1, 0), Water);
                        WaterTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1, 0), Water);
                        WaterTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1 - 1, 0), Water);
                        WaterTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1 - 1, 0), Water);
                    }
                }
            }
        }
        void LoadNodes()
        {
            int placedNodes = 0;
            int loop = 0;

            bool[,] nodesInSpot = new bool[16, 16];

            List<Vector2Int> possiblePlacements = _tileData.GetPossibleNodePlacements();

            while (placedNodes < possiblePlacements.Count)
            {
                loop++;
                if (loop > 10000)
                {
                    Debug.LogWarning("REACHED MAX NUMBER OF LOOPS!!!");
                    break;
                }

                if (possiblePlacements.Count == 0)
                    break;

                var randSpot = possiblePlacements.RandomListItem();
                var randOffsetX = UnityEngine.Random.Range(-0.5f, 0.5f);
                var randOffsetY = UnityEngine.Random.Range(-0.5f, 0.5f);

                var randX = randSpot.x;
                var randY = randSpot.y;

                var nodeToPlace = _tileData.TileSettings.GetNodePrefab(Seed.GetHashCode(), loop);
                if (nodeToPlace == null)
                    continue;
                placedNodes++;
                var node = Instantiate(nodeToPlace, NodesParent);
                _tileData.ResrouceNodesOnTile.Add(node.GetComponentInChildren<ResourceNodeHandler>(true).NodeData);
                node.transform.localPosition = new Vector3(randX * 2 + 1 + randOffsetX, (randY + 2) * 2 - 1 + randOffsetY, 0);
                nodesInSpot[randX, randY] = true;
                possiblePlacements.Remove(randSpot);

            }
            Debug.Log($"Placed {placedNodes} nodes with {loop} loops");
        }

        return true;
    }

    public async Task LoadTileAsync()
    {
        LoadTile();
        await Task.Yield();
    }

    public void LoadTileCorroutine(bool UseFasterLoading = false, bool HideLoading = true, bool ShowLogs = false)
    {
        if (_tileData == null)
            return;

        var renderers = new List<TilemapRenderer>();
        renderers.Add(IslandTiles.GetComponent<TilemapRenderer>());
        renderers.Add(WaterTiles.GetComponent<TilemapRenderer>());
        renderers.Add(GrassTiles.GetComponent<TilemapRenderer>());
        renderers.Add(DetailsTiles.GetComponent<TilemapRenderer>());
        if (HideLoading)
        {
            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            }
        }

        _tileData.Initialize(WIDTH, HEIGHT);

        var Seed = _tileData.Seed;
        var tileSettings = database.GetTileSettings(_tileData.TileSettings);

        //Tiles
        var Water = tileSettings.Water;
        var Grass = tileSettings.Grass;
        var CliffRT = tileSettings.Cliffs;

        //Noise
        var noise = _tileData.TryGetNoise();
        ClearTile();
        StartCoroutine(LoadTiles());
        IEnumerator LoadTiles()
        {
            //Load base

            /*
            IslandTiles.origin = Vector3Int.zero;
            IslandTiles.size = new Vector3Int(32,34);
            IslandTiles.ResizeBounds();
            IslandTiles.BoxFill(new Vector3Int(0, 0, 1), CliffRT, 0, 0, 32, 34);
            */

            if (UseFasterLoading)
            {
                var tiles = new TileBase[WIDTH * (HEIGHT + 2)];
                for (int x = 0; x < WIDTH; x++)
                {
                    for (int y = 0; y < HEIGHT + 2; y++)
                    {
                        tiles[(x * (WIDTH + 2)) + y] = CliffRT;
                    }
                }
                BoundsInt bounds = new(0, 0, 0, WIDTH, HEIGHT + 2, 1);
                IslandTiles.SetTilesBlock(bounds, tiles);

            }
            else
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    for (int y = 0; y < HEIGHT + 2; y++)
                    {
                        IslandTiles.SetTile(new Vector3Int(x, y, 0), CliffRT);
                        if (y % 10 == 0 && !UseFasterLoading)
                            yield return null;
                    }
                }
            }



            //Load Grass

            if (Grass != null)
            {
                var _grassArea = _tileData.TileSettings.GrassArea;

                for (int x = 0; x < noise.GetLength(0); x++)
                {
                    for (int y = 0; y < noise.GetLength(1); y++)
                    {
                        if (noise[x, y] < _grassArea)
                        {
                            GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1, 0), Grass);
                            GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1, 0), Grass);
                            GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1 - 1, 0), Grass);
                            GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1 - 1, 0), Grass);

                            try
                            {
                                if (noise[x + 1, y] > _grassArea)
                                    continue;
                                if (noise[x - 1, y] > _grassArea)
                                    continue;
                                if (noise[x, y - 1] > _grassArea)
                                    continue;
                                if (noise[x, y + 1] > _grassArea)
                                    continue;
                                if (noise[x + 1, y + 1] > _grassArea)
                                    continue;
                                if (noise[x - 1, y + 1] > _grassArea)
                                    continue;
                                if (noise[x - 1, y - 1] > _grassArea)
                                    continue;
                                if (noise[x + 1, y - 1] > _grassArea)
                                    continue;
                            }
                            catch (System.Exception)
                            {
                            }

                            if (y % 5 == 0 && !UseFasterLoading)
                                yield return null;

                        }
                    }
                }
            }


            //Load Water
            if (Water != null)
            {
                var _waterArea = _tileData.TileSettings.WaterArea;
                for (int x = 1; x < noise.GetLength(0) - 1; x++)
                {
                    for (int y = 1; y < noise.GetLength(1) - 1; y++)
                    {
                        if (noise[x, y] > _waterArea)
                        {
                            WaterTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1, 0), Water);
                            WaterTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1, 0), Water);
                            WaterTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1 - 1, 0), Water);
                            WaterTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1 - 1, 0), Water);
                        }
                        if (y % 5 == 0 && !UseFasterLoading)
                            yield return null;

                    }
                }
            }


            //Load Ndoes
            int placedNodes = 0;
            int loop = 0;

            bool[,] nodesInSpot = new bool[16, 16];

            List<Vector2Int> possiblePlacements = _tileData.GetPossibleNodePlacements();

            while (placedNodes < possiblePlacements.Count)
            {
                loop++;
                if (loop > 10000)
                {
                    Debug.LogWarning("REACHED MAX NUMBER OF LOOPS!!!");
                    break;
                }

                if (possiblePlacements.Count == 0)
                    break;

                var randSpot = possiblePlacements.RandomListItem();
                var randOffsetX = UnityEngine.Random.Range(-0.5f, 0.5f);
                var randOffsetY = UnityEngine.Random.Range(-0.5f, 0.5f);

                var randX = randSpot.x;
                var randY = randSpot.y;

                var nodeToPlace = _tileData.TileSettings.GetNodePrefab(Seed.GetHashCode(), loop);
                if (nodeToPlace == null)
                    continue;
                placedNodes++;
                var node = Instantiate(nodeToPlace, NodesParent);
                _tileData.ResrouceNodesOnTile.Add(node.GetComponentInChildren<ResourceNodeHandler>(true).NodeData);
                node.transform.localPosition = new Vector3(randX * 2 + 1 + randOffsetX, (randY + 2) * 2 - 1 + randOffsetY, 0);
                nodesInSpot[randX, randY] = true;
                possiblePlacements.Remove(randSpot);

            }
            if (ShowLogs)
                Debug.Log($"Placed {placedNodes} nodes with {loop} loops");

            //Load border
            SetBorders();

            //Load Navmesh
            NavMeshSurface surface = GetComponentInChildren<NavMeshSurface>();
            surface.hideEditorLogs = true;
            surface.BuildNavMeshAsync();

            if (HideLoading)
            {
                foreach (var renderer in renderers)
                {
                    renderer.enabled = true;
                }
            }


            OnTileLoad?.Invoke(this);
        }

    }

    public void ClearTile()
    {
        IslandTiles.ClearAllTiles();
        GrassTiles.ClearAllTiles();
        DetailsTiles.ClearAllTiles();
    }

    public void UnLoadTile()
    {
        ClearTile();
        NodesParent.KillChildren();
    }

    public void OpenSide(Direction side)
    {
        /* Collider positioning

                    1 2
                    0 3
         
        Colliders have 3 points
        Prioritizes right/left
                    2    
                    1 0

        Whole box looks like
                 p1 1 0 0 1 p2
                    2     2
                    2     2
                 p0 1 0 0 1 p3
         */
        switch (side)
        {
            case Direction.Left:

                SetPoint(Colliders[0], 2, 16.5f);
                SetPoint(Colliders[1], 2, 17.5f);
                break;
            case Direction.Right:
                SetPoint(Colliders[2], 2, 17.5f);
                SetPoint(Colliders[3], 2, 16.5f);
                break;
            case Direction.Up:

                SetPoint(Colliders[1], 0, 15.5f);
                SetPoint(Colliders[2], 0, 16.5f);
                break;
            case Direction.Down:

                SetPoint(Colliders[0], 0, 15.5f);
                SetPoint(Colliders[3], 0, 16.5f);
                break;
            default:
                break;
        }
    }

    void SetPoint(EdgeCollider2D collider, int pointPos, float newPos)
    {
        List<Vector2> editPoints = new();
        collider.GetPoints(editPoints);
        editPoints[pointPos] = new Vector2(pointPos == 2 ? editPoints[pointPos].x : newPos, pointPos == 2 ? newPos : editPoints[pointPos].y);
        collider.SetPoints(editPoints);
    }

    void SetBorders()
    {
        if (_tileData.TileSettings.TileBiome == TileBiome.Arctic)
            return;

        //Set Colliders

        //Bottom Left
        List<Vector2> editPoints = new();
        Colliders[0].GetPoints(editPoints);
        editPoints[0] = new Vector2(editPoints[0].x, editPoints[0].y + (_tileData.TileSettings.TileBiome == TileBiome.Desert ? 0.5f : -0.5f) + (_tileData.TileSettings.TileBiome == TileBiome.Arctic ? 0f : 0.25f));
        editPoints[1] = new Vector2(editPoints[1].x + 0.5f, editPoints[1].y + (_tileData.TileSettings.TileBiome == TileBiome.Desert ? 0.5f : -0.5f) + (_tileData.TileSettings.TileBiome == TileBiome.Arctic ? 0f : 0.25f));
        editPoints[2] = new Vector2(editPoints[2].x + 0.5f, editPoints[2].y);
        Colliders[0].SetPoints(editPoints);
        editPoints.Clear();

        //Top Left
        Colliders[1].GetPoints(editPoints);
        editPoints[0] = new Vector2(editPoints[0].x, editPoints[0].y - (_tileData.TileSettings.TileBiome == TileBiome.Arctic ? 0.5f : 0.75f));
        editPoints[1] = new Vector2(editPoints[1].x + 0.5f, editPoints[1].y - (_tileData.TileSettings.TileBiome == TileBiome.Arctic ? 0.5f : 0.75f));
        editPoints[2] = new Vector2(editPoints[2].x + 0.5f, editPoints[2].y);
        Colliders[1].SetPoints(editPoints);
        editPoints.Clear();

        //Top Right
        Colliders[2].GetPoints(editPoints);
        editPoints[0] = new Vector2(editPoints[0].x, editPoints[0].y - (_tileData.TileSettings.TileBiome == TileBiome.Arctic ? 0.5f : 0.75f));
        editPoints[1] = new Vector2(editPoints[1].x - 0.5f, editPoints[1].y - (_tileData.TileSettings.TileBiome == TileBiome.Arctic ? 0.5f : 0.75f));
        editPoints[2] = new Vector2(editPoints[2].x - 0.5f, editPoints[2].y);
        Colliders[2].SetPoints(editPoints);
        editPoints.Clear();

        //Bottom Right
        Colliders[3].GetPoints(editPoints);
        editPoints[0] = new Vector2(editPoints[0].x, editPoints[0].y + (_tileData.TileSettings.TileBiome == TileBiome.Desert ? 0.5f : -0.5f) + (_tileData.TileSettings.TileBiome == TileBiome.Arctic ? 0f : 0.25f));
        editPoints[1] = new Vector2(editPoints[1].x - 0.5f, editPoints[1].y + (_tileData.TileSettings.TileBiome == TileBiome.Desert ? 0.5f : -0.5f) + (_tileData.TileSettings.TileBiome == TileBiome.Arctic ? 0f : 0.25f));
        editPoints[2] = new Vector2(editPoints[2].x - 0.5f, editPoints[2].y);
        Colliders[3].SetPoints(editPoints);


    }

    public void CloseSide(Direction side)
    {
        switch (side)
        {
            case Direction.Left:

                SetPoint(Colliders[0], 2, 17);
                SetPoint(Colliders[1], 2, 17);
                break;
            case Direction.Right:
                SetPoint(Colliders[2], 2, 17);
                SetPoint(Colliders[3], 2, 17);
                break;
            case Direction.Up:

                SetPoint(Colliders[1], 0, 16);
                SetPoint(Colliders[2], 0, 16);
                break;
            case Direction.Down:

                SetPoint(Colliders[0], 0, 16);
                SetPoint(Colliders[3], 0, 16);
                break;
            default:
                break;
        }
    }

    [ContextMenu("Open Left")]
    void OpenLeft()
    {
        OpenSide(Direction.Left);
    }
    [ContextMenu("Open Right")]
    void OpenRight()
    {
        OpenSide(Direction.Right);
    }
    [ContextMenu("Open Bottom")]
    void OpenBottom()
    {
        OpenSide(Direction.Down);
    }
    [ContextMenu("Open Top")]
    void OpenTop()
    {
        OpenSide(Direction.Up);
    }
    [ContextMenu("Close Left")]
    void CloseLeft()
    {
        CloseSide(Direction.Left);
    }
    [ContextMenu("Close Right")]
    void CloseRight()
    {
        CloseSide(Direction.Right);
    }
    [ContextMenu("Close Bottom")]
    void CloseBottom()
    {
        CloseSide(Direction.Down);
    }
    [ContextMenu("Close Top")]
    void CLoseTop()
    {
        CloseSide(Direction.Up);
    }
}

public class TileConnectors
{
    public Transform TopConnector;
    public Transform LeftConnector;
    public Transform RightConnector;
    public Transform BottomConnector;

    public void AddConnector(Transform placedConnector, Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                if (LeftConnector != null)
                    UnityEngine.Object.Destroy(LeftConnector.gameObject);
                LeftConnector = placedConnector;
                return;
            case Direction.Right:
                if (RightConnector != null)
                    UnityEngine.Object.Destroy(RightConnector.gameObject);
                RightConnector = placedConnector;
                return;
            case Direction.Up:
                if (TopConnector != null)
                    UnityEngine.Object.Destroy(TopConnector.gameObject);
                TopConnector = placedConnector;
                return;
            case Direction.Down:
                if (BottomConnector != null)
                    UnityEngine.Object.Destroy(BottomConnector.gameObject);
                BottomConnector = placedConnector;
                return;
        }
    }

    public void RemoveConnector(Direction ConnectorDirection)
    {
        Transform connector;
        switch (ConnectorDirection)
        {
            case Direction.Left:
                connector = LeftConnector;
                break;
            case Direction.Right:
                connector = RightConnector;
                break;
            case Direction.Up:
                connector = TopConnector;
                break;
            case Direction.Down:
                connector = BottomConnector;
                break;
            default:
                connector = LeftConnector;
                break;
        }
        if (connector != null)
            UnityEngine.Object.Destroy(connector.gameObject);
    }

}
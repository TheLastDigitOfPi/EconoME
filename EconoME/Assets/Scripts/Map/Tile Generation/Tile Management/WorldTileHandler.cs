using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldTileHandler : MonoBehaviour
{
    [SerializeField] Tilemap IslandTiles;
    [SerializeField] Tilemap GrassTiles;
    [SerializeField] Tilemap DetailsTiles;

    [SerializeField] Transform NodesParent;

    const int HEIGHT = 32;
    const int WIDTH = 32;
    TileItem _tileData;

    //Helpers
    TileGenerationDatabase database { get { return ResourceWorldManager.Instance.database; } }


    public void Initialize(TileItem data)
    {
        _tileData = data;
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
                        GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1, 0), Water);
                        GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1, 0), Water);
                        GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1 - 1, 0), Water);
                        GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1 - 1, 0), Water);
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
                var node = Object.Instantiate(nodeToPlace, NodesParent);
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

    public void LoadTileCorroutine()
    {
        if (_tileData == null)
            return;


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
            for (int x = 0; x < WIDTH; x++)
            {
                for (int y = 0; y < HEIGHT + 2; y++)
                {
                    IslandTiles.SetTile(new Vector3Int(x, y, 0), CliffRT);
                    if (y % 5 == 0)
                        yield return null;
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

                            if (y % 5 == 0)
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
                            GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1, 0), Water);
                            GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1, 0), Water);
                            GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1 - 1, 0), Water);
                            GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1 - 1, 0), Water);
                        }
                        if (y % 5 == 0)
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
                var node = Object.Instantiate(nodeToPlace, NodesParent);
                _tileData.ResrouceNodesOnTile.Add(node.GetComponentInChildren<ResourceNodeHandler>(true).NodeData);
                node.transform.localPosition = new Vector3(randX * 2 + 1 + randOffsetX, (randY + 2) * 2 - 1 + randOffsetY, 0);
                nodesInSpot[randX, randY] = true;
                possiblePlacements.Remove(randSpot);

            }
            Debug.Log($"Placed {placedNodes} nodes with {loop} loops");
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
}
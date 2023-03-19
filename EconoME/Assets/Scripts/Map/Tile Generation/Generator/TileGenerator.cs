using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    [SerializeField] Tilemap IslandTiles;
    [SerializeField] Tilemap GrassTiles;
    [SerializeField] Tilemap DetailsTiles;
    [SerializeField] RuleTile Grass;
    [SerializeField] RuleTile Water;
    [SerializeField] RuleTile CliffRT;
    [SerializeField] List<Vector2> detailsChances = new();
    [SerializeField] List<TileBase> detailsOptions = new();
    [SerializeField] TileGenerationDatabase database;
    [SerializeField] int MaxNodes = 15;

    [SerializeField] GameObject ResourceNode;
    List<GameObject> _placedNodes = new();
    [field: SerializeField] public string Seed { get; private set; }

    [SerializeField] float grassArea = 0.6f;
    const float GROUNDAREA = 0.7f;
    [SerializeField, Range(0.7f, 1f)] float water;
    [SerializeField] bool UseWater = false;
    [SerializeField] bool GenerateDetails = false;
    const int TILEWIDTH = 32;
    const int TILEHEIGHT = 32;

    [ContextMenu("Generate Tile")]
    public void GenerateTile()
    {
        ClearTile();
        //First generate the tile island

        GenerateBase();

        //Generate Grass area

        var noise = PerlinNoiseGenerator.GeneratePerlinNoise2D(TILEWIDTH / 2, TILEHEIGHT / 2, Seed.GetHashCode(), scale: 3.5f);
        GenerateGrass(noise);

        //Generate Nodes

        int placedNodes = 0;
        int loop = 0;

        bool[,] nodesInSpot = new bool[16, 16];

        List<Vector2Int> possiblePlacements = new();

        for (int x = 1; x < 15; x++)
        {
            for (int y = 1; y < 15; y++)
            {
                //Check if this spot can contain a tree
                if (noise[x, y] > grassArea)
                    continue;

                //If one of its neighbors is invalid, skip this spot
                if (noise[x + 1, y] > grassArea)
                    continue;
                if (noise[x - 1, y] > grassArea)
                    continue;
                if (noise[x, y - 1] > grassArea)
                    continue;
                if (noise[x, y + 1] > grassArea)
                    continue;
                if (noise[x + 1, y + 1] > grassArea)
                    continue;
                if (noise[x - 1, y + 1] > grassArea)
                    continue;
                if (noise[x - 1, y - 1] > grassArea)
                    continue;
                if (noise[x + 1, y - 1] > grassArea)
                    continue;

                possiblePlacements.Add(new Vector2Int(x, y));
            }
        }

        while (placedNodes < MaxNodes)
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
            var randOffsetX = Random.Range(-0.5f, 0.5f);
            var randOffsetY = Random.Range(-0.5f, 0.5f);

            var randX = randSpot.x;
            var randY = randSpot.y;

            placedNodes++;
            var node = Instantiate(ResourceNode, transform);
            _placedNodes.Add(node);
            node.transform.localPosition = new Vector3(randX * 2 + 1 + randOffsetX, (randY + 2) * 2 - 1 + randOffsetY, 0);
            nodesInSpot[randX, randY] = true;
            possiblePlacements.Remove(randSpot);


        }
        Debug.Log($"Placed {placedNodes} nodes with {loop} loops");

        //Generate Water area
        if (!UseWater)
            return;
        GenerateWater(noise);

        if (!GenerateDetails)
            return;
        List<Vector2Int> detailsSpots = new();
        //generate details
        for (int x = 1; x < noise.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < noise.GetLength(1) - 1; y++)
            {
                foreach (var chance in detailsChances)
                {

                    if (noise[x, y].isBetweenMinInclusive(chance.x, chance.y))
                    {
                        detailsSpots.Add(new Vector2Int(x, y));
                        break;
                    }
                }

            }
        }

        System.Random rand = new(Seed.GetHashCode());
        foreach (var spot in detailsSpots)
        {
            var chosenDetail = detailsOptions[rand.Next(0, detailsOptions.Count)];

            var randDouble1 = (float)rand.NextDouble() / 2f;
            var randDouble2 = (float)rand.NextDouble() / 2f;
            var pos1 = rand.Next(0, 1) == 0 ? -1f : 1f;
            var pos2 = rand.Next(0, 1) == 0 ? -1f : 1f;
            var offSetX = randDouble1 * pos1;
            var offSetY = randDouble2 * pos2;
            TileChangeData tileChangeData = new()
            {
                position = new Vector3Int(spot.x * 2, (spot.y + 2) * 2 - 1, 0),
                tile = chosenDetail,
                transform = Matrix4x4.Translate(new Vector3(offSetX, offSetY, 0))
            };
            DetailsTiles.SetTile(tileChangeData, false);
        }


    }

    void GenerateWater(float[,] noise)
    {
        for (int x = 1; x < noise.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < noise.GetLength(1) - 1; y++)
            {
                if (noise[x, y] > water)
                {
                    GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1, 0), Water);
                    GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1, 0), Water);
                    GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1 - 1, 0), Water);
                    GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1 - 1, 0), Water);
                }
            }
        }
    }

    void GenerateGrass(float[,] noise)
    {
        for (int x = 0; x < noise.GetLength(0); x++)
        {
            for (int y = 0; y < noise.GetLength(1); y++)
            {
                if (noise[x, y] < grassArea)
                {
                    GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1, 0), Grass);
                    GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1, 0), Grass);
                    GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1 - 1, 0), Grass);
                    GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1 - 1, 0), Grass);

                    try
                    {
                        if (noise[x + 1, y] > grassArea)
                            continue;
                        if (noise[x - 1, y] > grassArea)
                            continue;
                        if (noise[x, y - 1] > grassArea)
                            continue;
                        if (noise[x, y + 1] > grassArea)
                            continue;
                        if (noise[x + 1, y + 1] > grassArea)
                            continue;
                        if (noise[x - 1, y + 1] > grassArea)
                            continue;
                        if (noise[x - 1, y - 1] > grassArea)
                            continue;
                        if (noise[x + 1, y - 1] > grassArea)
                            continue;
                    }
                    catch (System.Exception)
                    {
                    }


                }
            }
        }
    }

    void GenerateBase()
    {
        for (int x = 0; x < TILEWIDTH; x++)
        {
            for (int y = 0; y < TILEHEIGHT + 2; y++)
            {
                IslandTiles.SetTile(new Vector3Int(x, y, 0), CliffRT);
            }
        }
    }

    public void ClearTile()
    {
        Debug.Log("Clearing Tile");
        IslandTiles.ClearAllTiles();
        foreach (var node in _placedNodes)
        {
            DestroyImmediate(node);
        }
        _placedNodes.Clear();
        GrassTiles.ClearAllTiles();
        DetailsTiles.ClearAllTiles();
    }

    public void GenerateRandomTile()
    {
        Debug.Log("Generating Random Tile");
        ClearTile();

        Seed = Random.Range(0, 69420).ToString();
        GenerateTile();
    }

    [Header("Tile Settings")]
    [SerializeField] TileSettings settings;
    public void GenerateTileFromSettings()
    {
        ClearTile();
        Seed = settings.UseRandomSeed ? Random.Range(0, 69420).ToString() : settings.BaseSeed.ToString();
        var tileSettings = database.GetTileSettings(settings);
        Water = tileSettings.Water;
        Grass = tileSettings.Grass;
        CliffRT = tileSettings.Cliffs;

        var noise = PerlinNoiseGenerator.GeneratePerlinNoise2D(TILEWIDTH / 2, TILEHEIGHT / 2, Seed.GetHashCode(), scale: 3.5f);
        GenerateBase();
        if (Grass != null)
            GenerateGrass(noise);
        if (Water != null)
            GenerateWater(noise);

        //Generate Nodes

        int placedNodes = 0;
        int loop = 0;

        bool[,] nodesInSpot = new bool[16, 16];

        List<Vector2Int> possiblePlacements = new();

        for (int x = 1; x < 15; x++)
        {
            for (int y = 1; y < 15; y++)
            {
                //Check if this spot can contain a tree
                if (noise[x, y] > grassArea)
                    continue;

                //If one of its neighbors is invalid, skip this spot
                if (noise[x + 1, y] > grassArea)
                    continue;
                if (noise[x - 1, y] > grassArea)
                    continue;
                if (noise[x, y - 1] > grassArea)
                    continue;
                if (noise[x, y + 1] > grassArea)
                    continue;
                if (noise[x + 1, y + 1] > grassArea)
                    continue;
                if (noise[x - 1, y + 1] > grassArea)
                    continue;
                if (noise[x - 1, y - 1] > grassArea)
                    continue;
                if (noise[x + 1, y - 1] > grassArea)
                    continue;

                possiblePlacements.Add(new Vector2Int(x, y));
            }
        }

        while (placedNodes < MaxNodes)
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
            var randOffsetX = Random.Range(-0.5f, 0.5f);
            var randOffsetY = Random.Range(-0.5f, 0.5f);

            var randX = randSpot.x;
            var randY = randSpot.y;

            var nodeToPlace = settings.GetNodePrefab(Seed.GetHashCode(), loop);
            if (nodeToPlace == null)
                continue;
            placedNodes++;
            var node = Instantiate(nodeToPlace, transform);
            _placedNodes.Add(node);
            node.transform.localPosition = new Vector3(randX * 2 + 1 + randOffsetX, (randY + 2) * 2 - 1 + randOffsetY, 0);
            nodesInSpot[randX, randY] = true;
            possiblePlacements.Remove(randSpot);


        }
        Debug.Log($"Placed {placedNodes} nodes with {loop} loops");

    }

}

[System.Serializable]
public class TileBaseSettings
{
    [field: SerializeField] public TileBiome BiomeType { get; private set; }
    [field: SerializeField] public RuleTile Cliffs { get; private set; }
    [field: SerializeField] public RuleTile Grass { get; private set; }
    [field: SerializeField] public RuleTile Water { get; private set; }

}
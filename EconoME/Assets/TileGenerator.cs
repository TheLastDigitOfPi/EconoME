using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    [SerializeField] Tilemap IslandTiles;
    [SerializeField] Tilemap GrassTiles;

    [SerializeField] RuleTile Grass;
    [SerializeField] RuleTile Water;
    [SerializeField] RuleTile CliffRT;


    [SerializeField] int MaxNodes = 15;

    [SerializeField] GameObject ResourceNode;
    List<GameObject> _placedNodes = new();
    [field: SerializeField] public string Seed { get; private set; }

    const float GRASSAREA = 0.6f;
    const float GROUNDAREA = 0.7f;
    [SerializeField, Range(0.7f, 1f)] float water;
    [SerializeField] bool UseWater = false;
    const int TILEWIDTH = 32;
    const int TILEHEIGHT = 32;

    [ContextMenu("Generate Tile")]
    public void GenerateTile()
    {
        //First generate the tile island

        for (int x = 0; x < TILEWIDTH; x++)
        {
            for (int y = 0; y < TILEHEIGHT + 2; y++)
            {
                IslandTiles.SetTile(new Vector3Int(x, y, 0), CliffRT);
            }
        }

        //Generate Grass area

        var noise = PerlinNoiseGenerator.GeneratePerlinNoise2D(TILEWIDTH / 2, TILEHEIGHT / 2, Seed.GetHashCode(), scale: 3.5f);

        for (int x = 0; x < noise.GetLength(0); x++)
        {
            for (int y = 0; y < noise.GetLength(1); y++)
            {
                if (noise[x, y] < GRASSAREA)
                {
                    GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1, 0), Grass);
                    GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1, 0), Grass);
                    GrassTiles.SetTile(new Vector3Int(x * 2 + 1, (y + 2) * 2 - 1 - 1, 0), Grass);
                    GrassTiles.SetTile(new Vector3Int(x * 2, (y + 2) * 2 - 1 - 1, 0), Grass);
                }
            }
        }

        //Generate Nodes

        int placedNodes = 0;
        int loop = 0;

        bool[,] nodesInSpot = new bool[16, 16];
        while (placedNodes < MaxNodes)
        {
            loop++;
            if (loop > 10000)
            {
                Debug.LogWarning("REACHED MAX NUMBER OF LOOPS!!!");
                break;
            }
            //Pick a random spot
            int randX = Random.Range(1, 15);
            int randY = Random.Range(1, 15);

            if (nodesInSpot[randX, randY])
                continue;
            var randOffsetX = Random.Range(-0.5f, 0.5f);
            var randOffsetY = Random.Range(-0.5f, 0.5f);

            if (noise[randX, randY] < GRASSAREA - 0.20f)
            {
                placedNodes++;
                var node = Instantiate(ResourceNode, transform);
                _placedNodes.Add(node);
                node.transform.localPosition = new Vector3(randX * 2 + 1 + randOffsetX, (randY + 2) * 2 - 1 + randOffsetY, 0);
                nodesInSpot[randX, randY] = true;
            }


        }
        Debug.Log($"Placed {placedNodes} nodes");
        //Generate Water area
        if (!UseWater)
            return;
        for (int x = 0; x < noise.GetLength(0); x++)
        {
            for (int y = 0; y < noise.GetLength(1); y++)
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
    }

    public void GenerateRandomTile()
    {
        Debug.Log("Generating Random Tile");
        ClearTile();

        Seed = Random.Range(0, 69420).ToString();
        GenerateTile();
    }

}

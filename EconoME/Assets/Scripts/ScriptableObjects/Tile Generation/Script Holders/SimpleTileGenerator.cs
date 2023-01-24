using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Tile Generator", menuName = "ScriptableObjects/TileGeneration/Scripts/Tile Generator")]
public class SimpleTileGenerator : TileGeneratorObject
{
    [SerializeField] Vector2Int Size;

    [Header("Prefabs")]
    [Space(5)]
    public GameObject NewTilePrefab;

    [Header("Misc")]
    public int SaveNum;

    [SerializeField] TileGenPresetDatabase TilePresets;
    [SerializeField] ResourceNodeDatabase _nodeDatabase;
    [SerializeField] InteractablesDatabase _interactablesDatabase;
    [SerializeField] TileNodesSettingsDataBase NodeSettingsDatabase;
    [SerializeField] TileDatabase _tileDatabase;
    [SerializeField] TileSriptableObject BlankTileObject;

    public override InteractablesDatabase InteractablesDatabase { get { return _interactablesDatabase; } }
    public override ResourceNodeDatabase NodeDatabase { get { return _nodeDatabase; } }
    public override TileDatabase TileDatabase { get { return _tileDatabase; } }


    public override WorldTile GenerateTile(TileType type, int tier)
    {
        TileGenerationPreset TP = TilePresets.FindPreset(type, tier);
        if (TP == null) { Debug.LogWarning("Unable to find preset that matches input type!"); return null; }
        return GenerateTile(TP);
    }

    public override WorldTile GenerateTile(TileGenerationPreset preset, TileGenerationSettings settings = null)
    {
        if (!preset.isValid())
        {
            Debug.LogWarning("Invalid Preset Cannot make WorldTile object");
            return null;
        }

        if (settings == null)
        {
            settings = NodeSettingsDatabase.GetDefaultSetting(preset.Type, preset.Tier);
            if (settings == null) { return null; }
        }
        WorldTile TileData = new(BlankTileObject, Size, settings.Tier);


        GenerateBaseLayer(preset.BaseTile);
        GenerateDirt(preset.DirtBaseTile);
        GenerateWater(preset.WaterTile);
        GenerateResourceNodes(settings);
        GenerateDetailsLayer(preset.DetailTiles, preset.DirtBaseTile);


        void GenerateBaseLayer(CustomTile BaseTile)
        {
            TileData.BaseLayerTiles = new int[Size.x * Size.y];
            for (int i = 0; i < Size.x * Size.y; i++)
            {
                TileData.BaseLayerTiles[i] = BaseTile.TileID;
            }
        }
        void GenerateWater(CustomTile water)
        {

            System.Random rand = new();

            int AttemptsToMakeWater = rand.Next(4, 8);

            for (int i = 0; i < AttemptsToMakeWater; i++)
            {
                int RandPoolSize = rand.Next(1000);
                int Width = 0;
                int Height = 0;
                bool stop = false;
                switch (RandPoolSize)
                {
                    //Generate Large Pool
                    case int when (RandPoolSize.isBetween(0, 10)):
                        Width = rand.Next(12, 20);
                        Height = rand.Next(12, 20);
                        stop = true;
                        break;
                    //Generate Medium Pool
                    case int when (RandPoolSize.isBetween(10, 100)):
                        Width = rand.Next(4, 9);
                        Height = rand.Next(4, 9);
                        break;
                    //Generate Small Pool
                    case int when (RandPoolSize.isBetween(100, 400)):
                        Width = rand.Next(3, 5);
                        Height = rand.Next(3, 5);
                        break;
                    default:
                        break;
                }

                if (Width > 1 && Height > 1)
                {
                    int randomPosx = rand.Next(2, Size.x - Width - 2);
                    int randPosY = rand.Next(2, Size.y - Height - 2);

                    int FlatPos = randomPosx + (Size.y * randPosY);

                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            try
                            {
                                TileData.InteractableLayerTiles[FlatPos + x + (Size.y * y)] = water.TileID;
                            }
                            catch (System.Exception)
                            {
                            }
                        }
                    }
                }
                if (stop)
                {
                    break;
                }

            }


        }
        void GenerateDirt(CustomTile dirt)
        {
            System.Random rand = new();

            int AttemptsToPlaceDirt = rand.Next(20, 50);


            for (int i = 0; i < AttemptsToPlaceDirt; i++)
            {
                int Width = rand.Next(2, 4);
                int Height = rand.Next(2, 4);

                if (Width > 1 && Height > 1)
                {
                    int randomPosx = rand.Next(0, Size.x - Width);
                    int randPosY = rand.Next(0, Size.y - Height);

                    int FlatPos = randomPosx + (Size.y * randPosY);

                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            try
                            {
                                TileData.BaseLayerTiles[FlatPos + x + (Size.y * y)] = dirt.TileID;
                            }
                            catch (System.Exception)
                            {
                            }
                        }
                    }
                }
            }



        }
        void GenerateResourceNodes(TileGenerationSettings settings)
        {
            if (settings == null) { return; }
            if (settings.NodesCanGenerate.Count == 0) { return; }
            System.Random rand = new();
            int placedNodes = 0;
            TileData.resourceNodes = new();
            while (placedNodes < TileData.BaseMaxNodes)
            {
                foreach (var item in settings.NodesCanGenerate)
                {
                    if (item.Value.OddsOfSpawning > rand.NextDouble())
                    {
                        PlaceNode(item.Value, item.Key);
                    }
                }
            }

            void PlaceNode(ResourceNode.ResourceNodeSetting nodeSettings, CustomResourceNode current)
            {
                int randX = rand.Next(2, Size.x - 2);
                int randY = rand.Next(2, Size.y - 2);
                int FlatCoords = randX + (Size.y * randY);
                int AttemptsToFindGoodTile = 0;
                while (TileData.InteractableLayerTiles[FlatCoords] != 0)
                {
                    randX = rand.Next(2, Size.x - 2);
                    randY = rand.Next(2, Size.y - 2);
                    FlatCoords = randX + (Size.y * randY);
                    AttemptsToFindGoodTile++;

                    if (AttemptsToFindGoodTile > 40)
                    {
                        for (int x = 2; x < Size.x - 2; x++)
                        {
                            for (int y = 2; y < Size.y - 2; y++)
                            {
                                if (TileData.InteractableLayerTiles[FlatCoords] != 0)
                                {
                                    /* DEPRECATED UNTIL NEW TILE GENERATING SYSTEM
                                    TileData.resourceNodes.Add(new ResourceNode(
                                        current, nodeSettings, FlatCoords));
                                    placedNodes++;
                                    */
                                    return;
                                }
                            }
                        }
                        placedNodes = TileData.BaseMaxNodes;
                        return;

                    }

                }
                /*DEPRECATED UNTIL NEW TILE GENERATING SYSTEM
                 * TileData.resourceNodes.Add(new ResourceNode(current, nodeSettings, FlatCoords));
                */
                placedNodes++;
                return;
            }
        }
        void GenerateDetailsLayer(List<CustomTile> Details, CustomTile Dirt)
        {
            System.Random rand = new();
            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    bool Placeable = TileData.BaseLayerTiles[i + (Size.y * j)] != Dirt.TileID && TileData.InteractableLayerTiles[i + (Size.y * j)] == 0;
                    if (Placeable)
                    {

                        int RNG = rand.Next(0, 10);
                        if (RNG > 8)
                        {
                            int randDTile = rand.Next(0, Details.Count);
                            TileData.DetailsLayerTiles[i + (j * Size.y)] = Details[randDTile].TileID;
                        }
                    }
                }
            }
        }



        return TileData;
    }
}

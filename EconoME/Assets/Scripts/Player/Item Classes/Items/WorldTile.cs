using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldTile : Item
{
    public Vector2Int size;
    public int Shrink;
    public bool automated = false;
    public int mapPosx;
    public int mapPosy;
    public string TileName;
    public int Tier;


    //Node Generation Stats
    [SerializeField] int _baseMaxNodes = 30;
    [SerializeField] float NodeGenSpeed;

    public int BaseMaxNodes { get { return _baseMaxNodes; } }

    //Data of layers that can be converted to JSON
    public int[] BaseLayerTiles = null;
    public int[] InteractableLayerTiles = null;
    public int[] DetailsLayerTiles = null;

    //List of all interactable objects such as flowers or grass
    //[NonSerialized] public List<GameObject> interactables = new List<GameObject>();

    public List<InteractablesData> Interactables = new List<InteractablesData>();
    //List of resource nodes
    public List<ResourceNode> resourceNodes = new List<ResourceNode>();

    [NonSerialized] public WorldTileHandler PlacedTileHandler;
    public GameObject[] doorConnectors = new GameObject[4];



    public WorldTile(TileSriptableObject itemType) : base(itemType)
    {
    }

    public WorldTile(TileSriptableObject itemType, Vector2Int Size, int tier) : base(itemType)
    {
        this.size = Size;
        int Area = size.x * size.y;
        BaseLayerTiles = new int[Area];
        InteractableLayerTiles = new int[Area];
        DetailsLayerTiles = new int[Area];
        Tier = tier;

    }

    public WorldTile(WorldTile TD) : base(TD)
    {
        size = TD.size;
        Shrink = TD.Shrink;
        automated = TD.automated;
        mapPosx = TD.mapPosx;
        mapPosy = TD.mapPosy;
        TileName = TD.TileName;

        _baseMaxNodes = TD.BaseMaxNodes;
        NodeGenSpeed = TD.NodeGenSpeed;

        //Tilebase Data for JSON
        BaseLayerTiles = TD.BaseLayerTiles;
        InteractableLayerTiles = TD.InteractableLayerTiles;
        DetailsLayerTiles = TD.DetailsLayerTiles;

        //Interactables such as flowers
        Interactables = TD.Interactables;

        //Resource Nodes
        resourceNodes = TD.resourceNodes;
    }

    public override Item Duplicate()
    {
        return new WorldTile(this);
    }

}

public class TileItem : Item
{
    //Store the data for data that does not change
    //Background Tiles are stored based off Seed
    int _seed;

    //There should be some defined area for resource nodes
    int _resourceNodesSeed;

    TileGenerationSettings _generationSettings;
    
    //Resource Nodes currently on tile
    //Mobs currently on tile
    //Structures currently on tile

    ResourceNode _resourceNode;
    public const int width = 32;
    public const int height = 32;

    public TileItem()
    {

    }

    public TileItem GenerateNewTile(TileGenerationSettings settings)
    {
        TileItem tile = new();
        if (settings.UseRandomSeed)
        {
            System.Random rand = new();
            _seed = rand.Next(0, 69420).GetHashCode() / 69420;
            _resourceNodesSeed = rand.Next(0,69420).GetHashCode() /69420;
        }

        return tile;
    }

    void GenerateResourceNodes()
    {

    }


}



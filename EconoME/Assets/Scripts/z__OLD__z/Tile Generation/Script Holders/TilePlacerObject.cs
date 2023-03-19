using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Tile Placer", menuName = "ScriptableObjects/TileGeneration/Scripts/Tile Placer")]
public class TilePlacerObject : ScriptableObject
{
    /*
    [SerializeField] WorldMap map;

    public SerializableDictionary<Vector2Int, WorldTile> WorldTiles
    {
        get
        {
            if (map == null) { return _worldTiles; }
            return map.WorldTiles;
        }
        set
        {
            if (map == null) { _worldTiles = value; return; }
            map.WorldTiles = value;
        }
    }

    public SerializableDictionary<Vector2Int, WorldTile> _worldTiles = new();

    [SerializeField] WorldItemHandler WorldItemPrefab;
    [SerializeField] WorldTileHandler BlankTilePrefab;
    [SerializeField] GameObject TileSprite;

    [Space(10)]

    [SerializeField] TileSriptableObject SpawnTileType;
    [SerializeField] TileBiome TileTypeToSpawn;
    [SerializeField] TileGeneratorObject TileGenerator;

    [SerializeField] TileGenerationPreset PresetToGenerate;

    private void OnEnable()
    {
        ResetMap();
    }

    void ResetMap()
    {
        WorldTiles = new();
        WorldTile SpawnTile = new WorldTile(SpawnTileType);
        SpawnTile.TileName = "Spawn";
        //SpawnTile.MapType = -1;
        WorldTiles.Add(new Vector2Int(0, 0), SpawnTile);
        WorldTiles.Add(new Vector2Int(0, -1), SpawnTile);
        WorldTiles.Add(new Vector2Int(-1, 0), SpawnTile);
        WorldTiles.Add(new Vector2Int(-1, -1), SpawnTile);
    }

    public void PlaceTile(Vector2Int newTilePos, WorldTile tileData, bool isInstant = false, bool setToMap = true)
    {
        GameObject TileParent = GameObject.FindGameObjectWithTag("PlaceTileParent");
        if (TileParent == null) { return; }
        //Set data for tile
        tileData.mapPosx = newTilePos.x;
        tileData.mapPosy = newTilePos.y;
        if (setToMap)
        {
            WorldTiles.Add(newTilePos, tileData);
        }
        //Generate Tile
        //Instantiate tile
        var TileHandler = Instantiate(BlankTilePrefab, TileParent.transform);

        //Set scale to 0 until sprite is full

        TileHandler.transform.localScale = Vector3.zero;

        Vector3Int NewTilePos = new Vector3Int(
            (newTilePos.x * tileData.size.x) + (newTilePos.x * 2),
            (newTilePos.y * tileData.size.y) + (newTilePos.y * 2),
             1);

        TileHandler.transform.position = NewTilePos;

        tileData.Shrink = 1;
        tileData.PlacedTileHandler = TileHandler;
        TileHandler.data = tileData;

        TileHandler.PlaceTile(new PlaceTileData(newTilePos, TileGenerator));


        #region Add Tile Connections
        //Check Nearby Tiles for connection

        List<Vector2Int> CheckPos = new();

        CheckPos.Add(Vector2Int.up);
        CheckPos.Add(Vector2Int.right);
        CheckPos.Add(Vector2Int.down);
        CheckPos.Add(Vector2Int.left);

        for (int i = 0; i < CheckPos.Count; i++)
        {
            if (WorldTiles.ContainsKey(newTilePos + CheckPos[i]))
            {
                TileHandler.UpdateNewConnections(CheckPos[i], WorldTiles[newTilePos + CheckPos[i]].PlacedTileHandler);
            }

        }
        #endregion

        //Generate Sprite on ground
        GameObject tempTileSprite = Instantiate(TileSprite);
        tempTileSprite.GetComponent<SpriteRenderer>().sprite = tileData.Icon;
        tempTileSprite.transform.localScale = new Vector3(1 / 32, 1 / 32, 1);
        tempTileSprite.transform.position = NewTilePos + new Vector3Int(1, 1, 0);


        #region Grow New Tile
        TileHandler.StartGrowTile(tempTileSprite, isInstant);
        #endregion

    }

    public WorldTileHandler GetTilePicture(WorldTile TileData)
    {
         GameObject TileParent = GameObject.FindGameObjectWithTag("PlaceTileParent");
        if (TileParent == null) { return null; }
        WorldTileHandler handler = Instantiate(BlankTilePrefab, TileParent.transform);

        handler.data = TileData;

        handler.OnTilemapLoaded += () => { handler.getMapImage(); };


        handler.PlaceTile(new PlaceTileData(new Vector2Int(0,0), TileGenerator));


        return handler;

    }

    internal void RemoveTileAt(Vector2Int pos)
    {
        WorldTiles.Remove(pos);

        //Check for Connectors
        if (WorldTiles.ContainsKey(pos + Vector2Int.up))
        {
            Destroy(WorldTiles[pos + Vector2Int.up].doorConnectors[2]);
            WorldTiles[pos + Vector2Int.up].doorConnectors[2] = null;
        }
        if (WorldTiles.ContainsKey(pos + Vector2Int.right))
        {
            Destroy(WorldTiles[pos + Vector2Int.right].doorConnectors[3]);
            WorldTiles[pos + Vector2Int.right].doorConnectors[3] = null;
        }
        if (WorldTiles.ContainsKey(pos + Vector2Int.down))
        {
            Destroy(WorldTiles[pos + Vector2Int.down].doorConnectors[0]);
            WorldTiles[pos + Vector2Int.down].doorConnectors[0] = null;
        }
        if (WorldTiles.ContainsKey(pos + Vector2Int.left))
        {
            Destroy(WorldTiles[pos + Vector2Int.left].doorConnectors[1]);
            WorldTiles[pos + Vector2Int.left].doorConnectors[1] = null;
        }



    }

    internal void PlaceChunkIntoWorld(Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var pos = new Vector2Int(-x + size.x / 2, -y + size.y / 2);
                if (!WorldTiles.ContainsKey(pos))
                {
                    var TileMade = TileGenerator.GenerateTile(PresetToGenerate);
                    if (TileMade != null)
                    {
                        PlaceTile(pos, TileMade, isInstant: true);
                    }
                }
            }
        }
    }
    /*
    public void PlaceWorld()
    {
        ClearWorld(resetMap:false);
        foreach (var item in WorldTiles)
        {
            if(item.Value.itemType == SpawnTileType){continue;}
            PlaceTile(item.Key, item.Value, isInstant:true);
        }
    }
    
    internal void ClearWorld(bool resetMap = true)
    {

        GameObject TileParent = GameObject.FindGameObjectWithTag("PlaceTileParent");
        if (TileParent == null) { return; }

        TileParent.transform.KillChildren();
        
        if (resetMap)
        {
            ResetMap();
        }
    }

    void CreateTileAtPos(int x, int y)
    {
        if (WorldTiles.ContainsKey(new Vector2Int(x, y))) { return; }
        var TileMade = TileGenerator.GenerateTile(PresetToGenerate);
        if (TileMade == null) { return; }

        PlaceTile(new Vector2Int(x, y), TileMade, isInstant: true);
    }
    */
}

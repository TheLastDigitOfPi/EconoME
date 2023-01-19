using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetailsPlacer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Destroy(this);
    }


    [SerializeField] List<RandomTileObjectSpecs> objectsToPlace = new();
    [SerializeField] TileBase grassTile;
    [SerializeField] Tilemap gorundTilemap;
    [SerializeField] Tilemap detailsTileMap;

    [ContextMenu("Generate Terrain Details")]
    public void GenerateTerrainDetails()
    {

        foreach (var tilePos in gorundTilemap.cellBounds.allPositionsWithin)
        {
            if (gorundTilemap.GetTile(tilePos) == grassTile)
            {
                var randObject = objectsToPlace.RandomListItem();

                float chance = Random.Range(0, 100f);
                //If hit chance to spawn
                if (chance < randObject.chanceToSpawn)
                {
                    detailsTileMap.SetTile(tilePos, randObject.tileToPlace);
                }

            }
        }

    }


    [ContextMenu("Reset Terrain")]
    public void ResetTerrain()
    {
        detailsTileMap.ClearAllTiles();
    }

    [ContextMenu("Reset Ground")]
    public void ResetGround()
    {
        foreach (var tilePos in gorundTilemap.cellBounds.allPositionsWithin)
        {
            if (isDetailsTile(gorundTilemap.GetTile(tilePos)))
            {
                gorundTilemap.SetTile(tilePos, grassTile);
            }
        }
    }

    bool isDetailsTile(TileBase tile)
    {
        foreach (var item in objectsToPlace)
        {
            if (item.tileToPlace == tile)
            {
                return true;
            }
        }

        return false;
    }


}

[System.Serializable]
public class RandomTileObjectSpecs
{
    public TileBase tileToPlace;
    [Range(0, 100)] public float chanceToSpawn;
}
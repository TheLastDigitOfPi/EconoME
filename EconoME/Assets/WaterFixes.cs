using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterFixes : MonoBehaviour
{
    [SerializeField] Tilemap WaterTilemap;
    [SerializeField] Tilemap GroundTilemap;
    [SerializeField] TileBase WaterRuleTile;
    [SerializeField] TileBase WaterFlatTile;

    [ContextMenu("Fix Map")]
    void FixMap()
    {
        foreach (var position in GroundTilemap.cellBounds.allPositionsWithin)
        {
            if(GroundTilemap.GetTile(position) == WaterRuleTile)
            {
                WaterTilemap.SetTile(position, WaterRuleTile);
                GroundTilemap.SetTile(position, null);
            }
            if(GroundTilemap.GetTile(position) == WaterFlatTile)
            {
                WaterTilemap.SetTile(position, WaterRuleTile);
                GroundTilemap.SetTile(position, null);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainSmoother : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] Tilemap tilemap;

    [Space(5)]
    [Header("Flat Textures")]
    [SerializeField] TileBase flatGrass;
    [SerializeField] TileBase flatDarkGrass;
    [SerializeField] TileBase flatDirt;
    [SerializeField] TileBase flatWater;

    [Space(5)]
    [Header("Dark Grass")]
    [SerializeField] TileBase darkGrassRuleTile;
    [SerializeField] TileBase darkGrass_cornerTL;
    [SerializeField] TileBase darkGrass_cornerTR;
    [SerializeField] TileBase darkGrass_cornerBL;
    [SerializeField] TileBase darkGrass_cornerBR;
    [SerializeField] TileBase darkGrass_T;
    [SerializeField] TileBase darkGrass_R;
    [SerializeField] TileBase darkGrass_L;
    [SerializeField] TileBase darkGrass_B;
    [SerializeField] TileBase darkGrass_turnTL;
    [SerializeField] TileBase darkGrass_turnTR;
    [SerializeField] TileBase darkGrass_turnBL;
    [SerializeField] TileBase darkGrass_turnBR;

    [Space(5)]
    [Header("Dirt")]
    [SerializeField] TileBase dirtRuleTile;
    [SerializeField] TileBase dirt_cornerTL;
    [SerializeField] TileBase dirt_cornerTR;
    [SerializeField] TileBase dirt_T;
    [SerializeField] TileBase dirt_R;
    [SerializeField] TileBase dirt_L;
    [SerializeField] TileBase dirt_BL;
    [SerializeField] TileBase dirt_BR;
    [SerializeField] TileBase dirt_B;

    bool isValidGrass(TileBase tile)
    {
        return (tile == darkGrass_L || tile == darkGrass_R || tile == darkGrass_T || tile == darkGrass_B || tile == darkGrass_cornerBR || tile == darkGrass_cornerBL
                || tile == darkGrass_cornerTR || tile == darkGrass_cornerTL || tile == darkGrass_turnBL || tile == darkGrass_turnBR || tile == darkGrass_turnTL || tile == darkGrass_turnTR || tile == flatGrass);
    }

    [ContextMenu("Smooth Terrain (Probably Laggy)")]
    public void SmoothTerrain()
    {
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            var tile = tilemap.GetTile(position);
            switch (tile)
            {
                case var _ when tile == flatDarkGrass:
                    FlatDarkGrassRule(position);
                    break;
                default:
                    break;
            }
        }

        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            var tile = tilemap.GetTile(position);
            switch (tile)
            {
                case var _ when tile == flatDarkGrass:
                    FixCorners(position);
                    break;
                default:
                    break;
            }
        }

        void FixCorners(Vector3Int pos)
        {
            if (isValidGrass(tilemap.GetTile(pos + Vector3Int.down)))
            {
                if (isValidGrass(tilemap.GetTile(pos + Vector3Int.down + Vector3Int.left)) && isValidGrass(tilemap.GetTile(pos + Vector3Int.left)))
                {
                    tilemap.SetTile(pos + Vector3Int.down + Vector3Int.left, darkGrass_cornerTR);
                }
                if (isValidGrass(tilemap.GetTile(pos + Vector3Int.down + Vector3Int.right)) && isValidGrass(tilemap.GetTile(pos + Vector3Int.right)))
                {
                    tilemap.SetTile(pos + Vector3Int.down + Vector3Int.right, darkGrass_cornerTL);
                }

            }
        }

        void FlatDarkGrassRule(Vector3Int pos)
        {

            if (tilemap.GetTile(pos + Vector3Int.down) == flatGrass)
            {
                tilemap.SetTile(pos + Vector3Int.down, darkGrass_T);
                if (isValidGrass(tilemap.GetTile(pos + Vector3Int.down + Vector3Int.left)) && isValidGrass(tilemap.GetTile(pos + Vector3Int.left)))
                {
                    tilemap.SetTile(pos + Vector3Int.down + Vector3Int.left, darkGrass_cornerTR);
                }
                if (isValidGrass(tilemap.GetTile(pos + Vector3Int.down + Vector3Int.right)) && isValidGrass(tilemap.GetTile(pos + Vector3Int.right)))
                {
                    tilemap.SetTile(pos + Vector3Int.down + Vector3Int.right, darkGrass_cornerTL);
                }

            }
            if (tilemap.GetTile(pos + Vector3Int.up) == flatGrass)
            {
                tilemap.SetTile(pos + Vector3Int.up, darkGrass_B);
                if (isValidGrass(tilemap.GetTile(pos + Vector3Int.up + Vector3Int.left)) && isValidGrass(tilemap.GetTile(pos + Vector3Int.left)))
                {
                    tilemap.SetTile(pos + Vector3Int.up + Vector3Int.left, darkGrass_cornerBR);
                }
                if (isValidGrass(tilemap.GetTile(pos + Vector3Int.up + Vector3Int.right)) && isValidGrass(tilemap.GetTile(pos + Vector3Int.right)))
                {
                    tilemap.SetTile(pos + Vector3Int.up + Vector3Int.right, darkGrass_cornerBL);
                }
            }

            if (isValidGrass(tilemap.GetTile(pos + Vector3Int.left)))
            {
                tilemap.SetTile(pos + Vector3Int.left, darkGrass_R);
                if (tilemap.GetTile(pos + Vector3Int.left + Vector3Int.down) == flatDarkGrass)
                {
                    tilemap.SetTile(pos + Vector3Int.left, darkGrass_turnBR);
                }
                if (tilemap.GetTile(pos + Vector3Int.left + Vector3Int.up) == flatDarkGrass)
                {
                    tilemap.SetTile(pos + Vector3Int.left, darkGrass_turnTR);
                }
            }
            if (isValidGrass(tilemap.GetTile(pos + Vector3Int.right)))
            {
                tilemap.SetTile(pos + Vector3Int.right, darkGrass_L);
                if (tilemap.GetTile(pos + Vector3Int.right + Vector3Int.down) == flatDarkGrass)
                {
                    tilemap.SetTile(pos + Vector3Int.right, darkGrass_turnBL);
                }
                if (tilemap.GetTile(pos + Vector3Int.right + Vector3Int.up) == flatDarkGrass)
                {
                    tilemap.SetTile(pos + Vector3Int.right, darkGrass_turnTL);
                }
            }


        }
        /*
        TileBase[] tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        foreach (var tile in tiles)
        {
            if (tile)
                Debug.Log("Found tile " + tile.name);
            switch (tile)
            {
                case var _ when tile == dirtRuleTile:
                    Debug.Log("Found Dirt Rule Tile");
                    break;
                default:
                    break;
            }
        }
        */
    }

    [ContextMenu("Clear Edges")]
    public void ClearEdges()
    {
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            var tile = tilemap.GetTile(position);
            if (tile == darkGrass_L || tile == darkGrass_R || tile == darkGrass_T || tile == darkGrass_B || tile == darkGrass_cornerBR || tile == darkGrass_cornerBL
                || tile == darkGrass_cornerTR || tile == darkGrass_cornerTL || tile == darkGrass_turnBL || tile == darkGrass_turnBR || tile == darkGrass_turnTL || tile == darkGrass_turnTR
                )
            {
                tilemap.SetTile(position, flatGrass);
            }
        }
    }

    [ContextMenu("Clear Rule Tiles")]
    public void ClearRuleTiles()
    {
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            var tile = tilemap.GetTile(position);
            if (tile == darkGrassRuleTile)
            {
                tilemap.SetTile(position, flatGrass);
            }
        }
    }

    [ContextMenu("Clean up small paths")]
    public void ClearSmallPaths()
    {
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            var tile = tilemap.GetTile(position);
            if (tile == flatGrass)
            {
                if (tilemap.GetTile(position + Vector3Int.up) == flatDarkGrass && (tilemap.GetTile(position + Vector3Int.down) == flatDarkGrass))
                {
                    tilemap.SetTile(position, flatDarkGrass);
                }
                if (tilemap.GetTile(position + Vector3Int.left) == flatDarkGrass && (tilemap.GetTile(position + Vector3Int.right) == flatDarkGrass))
                {
                    tilemap.SetTile(position, flatDarkGrass);
                }


            }
        }
    }
}

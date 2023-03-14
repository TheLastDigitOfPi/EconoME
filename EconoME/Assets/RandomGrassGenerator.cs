using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomGrassGenerator : MonoBehaviour
{
    [SerializeField] RuleTile grass;
    [SerializeField] Tilemap grassLayer;
    [SerializeField] PerlinNoiseGenerator generator;
    [SerializeField, Range(0, 1)] float Threshold = 0.8f;
    public void Generate()
    {
        var noise = generator.GeneratePelinData();

        for (int x = 0; x < noise.GetLength(0); x++)
        {
            for (int y = 0; y < noise.GetLength(1); y++)
            {
                if (noise[x, y] < Threshold)
                {
                    Vector3Int pos = new(x, y, 0);
                    if (!grassLayer.HasTile(pos))
                        grassLayer.SetTile(pos, grass);
                }
            }
        }

        CleanUpGrass();

        void CleanUpGrass()
        {
            var bounds = grassLayer.cellBounds;

            for (int x = 0; x < noise.GetLength(0); x++)
            {
                for (int y = 0; y < noise.GetLength(1); y++)
                {

                    /* Tile positions
                        0 1 2
                        7 x 3
                        6 5 4
                     */

                    Vector3Int[] positions = new Vector3Int[8];

                    Vector3Int pos = new(x, y, 0);

                    if (grassLayer.GetTile(pos) != grass)
                        continue;

                    positions[0] = new(x - 1, y + 1, 0);
                    positions[1] = new(x, y + 1, 0);
                    positions[2] = new(x + 1, y + 1, 0);
                    positions[3] = new(x + 1, y, 0);
                    positions[4] = new(x + 1, y - 1, 0);
                    positions[5] = new(x, y - 1, 0);
                    positions[6] = new(x - 1, y - 1, 0);
                    positions[7] = new(x - 1, y, 0);

                    if (CheckCorner(1) || CheckCorner(3) || CheckCorner(5) || CheckCorner(7))
                        continue;
                    Debug.Log("removed a bad tile");
                    grassLayer.SetTile(pos, null);

                    bool CheckPos(Vector3Int testPos)
                    {
                        if (grassLayer.GetTile(testPos) == grass)
                            return true;
                        return false;
                    }

                    bool CheckCorner(int arrayPos)
                    {
                        if (!CheckPos(positions[arrayPos]))
                            return false;
                        if (!CheckPos(positions[arrayPos == 7 ? 0 : arrayPos + 1]))
                            return false;
                        if (!CheckPos(positions[arrayPos == 7 ? 1 : arrayPos + 2]))
                            return false;
                        return true;
                    }
                }
            }

        }

    }

    public void ResetGrass()
    {
        grassLayer.ClearAllTiles();
    }
}

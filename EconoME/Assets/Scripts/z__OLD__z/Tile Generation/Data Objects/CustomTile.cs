using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "New Custom Tile", menuName = "ScriptableObjects/TileGeneration/DataObjects/Custom Tile")]
[Serializable]
public class CustomTile : ScriptableObject
{
    public TileBase tile;
    public int TileID;
    public bool DetailsTile;
}

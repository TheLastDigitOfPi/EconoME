using UnityEngine;

public struct PlaceTileData
{
    public Vector2Int MapPosition;
    public TileDatabase tileDatabase;
    public ResourceNodeDatabase nodesDatabase;
    public InteractablesDatabase interactablesDatabase;

    public PlaceTileData(Vector2Int mapPosition, TileGeneratorObject tileGeneratorUsed)
    {
        MapPosition = mapPosition;
        tileDatabase = tileGeneratorUsed.TileDatabase;
        nodesDatabase = tileGeneratorUsed.NodeDatabase;
        interactablesDatabase = tileGeneratorUsed.InteractablesDatabase;
    }
}

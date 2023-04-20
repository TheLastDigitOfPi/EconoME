using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PillarHandler : MonoBehaviour, IAmInteractable
{
    Vector2Int _tilePosition;
    Vector2Int _neighborTilePosition;


    public void Initialize(Vector2Int tilePosition, Vector2Int neightborTilePosition)
    {
        _tilePosition = tilePosition;
        _neighborTilePosition = neightborTilePosition; 
    }

    public bool OnRaycastHit(PlayerMovementController owner, Collider2D collider)
    {
        if (!HotBarHandler.GetCurrentSelectedItem(out var item))
            return false;

        if(item is not TileItem)
            return false;

        var tile = item as TileItem;

        WorldTileManager.Instance.TryPlaceTile(tile, _neighborTilePosition, out _);
        Debug.Log("Pillar clicked with tile");
        return true;
    }

}

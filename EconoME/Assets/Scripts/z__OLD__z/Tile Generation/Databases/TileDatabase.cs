using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu(fileName = "New Tile Database", menuName = "ScriptableObjects/TileGeneration/Databases/Tile Database")]
public class TileDatabase : ScriptableObject
{
    [SerializeField] CustomTile[] TilesInDatabase;

    public TileBase FindTile(int id)
    {
        var FoundTile = TilesInDatabase.FirstOrDefault(t => t.TileID == id);

        if (FoundTile == null)
        {
            Debug.LogWarning($"Failed to find Tilebase for Search ID: {id}");
            return null;
        }

        return FoundTile.tile;
    }

    public int FindID(TileBase searchTile)
    {
        var FoundTile = TilesInDatabase.FirstOrDefault(t => t.tile == searchTile);

        if (FoundTile == null)
        {
            Debug.LogWarning($"Failed to find Tile ID for Search Tile of {searchTile.name}");
            return -1;
        }

        return FoundTile.TileID;
    }


}

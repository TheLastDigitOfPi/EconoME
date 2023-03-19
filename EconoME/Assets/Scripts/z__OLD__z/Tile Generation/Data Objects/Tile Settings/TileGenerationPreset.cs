using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Tile Generation Preset", menuName = "ScriptableObjects/TileGeneration/DataObjects/Tile Generation Preset")]
public class TileGenerationPreset : ScriptableObject
{
    [Header("Type of Biome")]
    [Space(5)]
    [SerializeField] TileBiome _type;
    [SerializeField] int _tier;

    public TileBiome Type {get{return _type;}}
    public int Tier {get{return _tier;}}


    [Header("Tiles")]
    [Space(5)]
    [SerializeField] CustomTile _baseTile;
    [SerializeField] CustomTile _dirtBaseTile;
    [SerializeField] CustomTile _waterTile;
    [SerializeField] List<CustomTile> _detailTiles;

    public CustomTile BaseTile { get { return _baseTile; } }
    public CustomTile DirtBaseTile{ get { return _dirtBaseTile; } }
    public CustomTile WaterTile{ get { return _waterTile; } }
    public List<CustomTile> DetailTiles{ get { return _detailTiles; } }
    
    [Header("ResourceNodes")]
    [Space(5)]
    public List<CustomResourceNode> ResourceNodes;

    public bool isValid()
    {
        if(_baseTile == null){return false;}
        if(_dirtBaseTile == null){return false;}
        if(_waterTile == null){return false;}
        if(_detailTiles == null){return false;}

        return true;
    }

}


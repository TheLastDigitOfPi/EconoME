using UnityEngine.Rendering;
using System;

[Serializable]
public class TileGenerationSettings
{
    public TileType TileType {get; private set;}
    public int Tier {get; private set;}
    public int BaseMaxNodes {get; private set;}
    public SerializedDictionary<CustomResourceNode, ResourceNode.ResourceNodeSetting> NodesCanGenerate = new();
    public CustomResourceNode AddKeyToDictionary;
    public ResourceNode.ResourceNodeSetting AddValueToDictionary;
    public int BaseSeed { get; private set; }
    public int ResourceNodeSeed { get; private set; }
    public bool UseRandomSeed { get; private set; } = true;

}
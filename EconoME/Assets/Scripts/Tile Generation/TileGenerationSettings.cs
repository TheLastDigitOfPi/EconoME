using UnityEngine.Rendering;
using System;

[Serializable]
public class TileGenerationSettings
{
    public TileType TileType;
    public int Tier;
    public int BaseMaxNodes;
    public SerializedDictionary<CustomResourceNode, ResourceNode.ResourceNodeSetting> NodesCanGenerate = new();
    public CustomResourceNode AddKeyToDictionary;
    public ResourceNode.ResourceNodeSetting AddValueToDictionary;

}
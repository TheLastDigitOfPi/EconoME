using UnityEngine;
using System;

[Serializable]
public class ResourceNode
{
    public int health;
    public ResourceNodeType NodeType;
    public ToolType ToolNeeded;
    public int NodePrefabID;
    public int Tier;
    public int PopThreshold { get; set; }
    public int RemainingThreshold { get; set; }
    public int MaxPops = 6;
    public int DroppedItemsPerPop = 10;
    public int PopsLeft { get; set; }
    public int TilePos;


    public ResourceNode(CustomResourceNode Node)
    {
        ToolNeeded = Node.RequiredTool;
        NodeType = Node.NodeType;
        NodePrefabID = Node.NodeDatabaseID;
    }

    public ResourceNode(CustomResourceNode Node, ResourceNodeSetting settings, int tilePos)
    {
         ToolNeeded = Node.RequiredTool;
        NodeType = Node.NodeType;
        NodePrefabID = Node.NodeDatabaseID;
        TilePos = tilePos;

        RandomizeStats(settings);

        void RandomizeStats(ResourceNodeSetting settings)
        {
            System.Random rand = new();
            health = rand.Next(settings.MinMaxHealth.x, settings.MinMaxHealth.y);
            PopThreshold = rand.Next(settings.MinMaxThreshold.x, settings.MinMaxThreshold.y);
            DroppedItemsPerPop = rand.Next(settings.MinMaxItemsPerPop.x, settings.MinMaxItemsPerPop.y);
        }
    }

    public Vector3 getTilePosition(Vector2Int sizeOfMap)
    {
        int y = TilePos / sizeOfMap.y;
        int x = TilePos % sizeOfMap.x;
        return new Vector3(-x + sizeOfMap.x / 2, -y + sizeOfMap.y / 2, 0);
    }

    [Serializable]
    public struct ResourceNodeSetting
    {
        [Range(0, 1f)]
        public float OddsOfSpawning;
        public Vector2Int MinMaxHealth;
        public Vector2Int MinMaxThreshold;
        public Vector2Int MinMaxItemsPerPop;

    }


}





using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ResourceNode
{

    public int TilePos;

    //Node Characteristics (tier, type, etc)

    public ResourceNodeType NodeType;
    public ToolType ToolNeeded;
    public int NodePrefabID;
    public int NodeTier { get; private set; }

    //Base Drop Settings
    [field: Header("Drop Settings")]
    [field: Space(10)]
    /// <summary>
    /// Max number of drops a node will provide before breaking
    /// </summary>
    [field: SerializeField] public int MaxDrops { get; private set; }
    /// <summary>
    /// The amount of damage needed to be done to the node in order to gain a single drop
    /// </summary>
    [field: SerializeField]public int DamagePerDrop { get; private set; }
    /// <summary>
    /// The time is takes to use a tool with speed 1 on the node in seconds
    /// </summary>
    [field: SerializeField]public float TimeToHarvest { get; private set; } = 1;
    /// <summary>
    /// The range for number of items that will be produced on a single drop
    /// </summary>
    [field: SerializeField] Vector2Int ItemCountPerDrop {get; set;}
    /// <summary>
    /// The number of possible drops remanining on the node
    /// </summary>
    [field: SerializeField] public int RemainingDrops { get; private set; }



    //Helpers
    public bool ValidNode
    {
        get
        {
            if (MaxDrops <= 0 || RemainingDrops <= 0)
            {
                Debug.LogWarning("Max or Remaining drops is 0 or less, node is not set or should be destroyed");
                return false;
            }
            if (DamagePerDrop <= 0)
            {
                Debug.LogWarning("Invalid Damager per drop");
                return false;
            }
            if (ItemCountPerDrop.x <= 0 || ItemCountPerDrop.y < ItemCountPerDrop.x)
            {
                Debug.LogWarning("Invalid Item Count Per Drop");
                return false;
            }
            return true;
        }
    }
    public bool CorrectToolUsed(ToolBase toolUsed)
    {
        if (toolUsed.ToolType != ToolNeeded || toolUsed.ToolTier < NodeTier)
        {
            return false;
        }
        return true;
    }

    public float RemainingPercentHealth { get { return RemainingDrops > 0 ? (float)RemainingDrops / MaxDrops : 0; } }

    //Local fields
    int _damageTillNextDrop;

    /// <summary>
    /// Attempts to get number of items dropped on a hit with a tool
    /// </summary>
    /// <param name="toolUsed">The tool used on the node, must meet node tool requirements</param>
    /// <param name="DropCount">A list of drop counts of the resource</param>
    /// <param name="BrokeNode">Returns true if the node broke during the hit</param>
    /// <returns>Returns true if the node was able to be hit</returns>
    public bool AttemptHit(ToolBase toolUsed, out List<int> DropCount, out bool BrokeNode)
    {
        BrokeNode = false;
        DropCount = new();
        if (!CorrectToolUsed(toolUsed))
            return false;

        //Validate members not possibly set due to unity serialization
        if (_damageTillNextDrop <= 0)
            _damageTillNextDrop = DamagePerDrop;

        //Get amount of drops
        int toolDamage = toolUsed.Damage;

        int fullDrops = 0;
        while (toolDamage >= 0)
        {
            //If the damage amount is enough to get a drop, get it
            if (toolDamage >= _damageTillNextDrop)
            {
                toolDamage -= _damageTillNextDrop;
                fullDrops++;
                RemainingDrops--;
                _damageTillNextDrop = DamagePerDrop;

                //If we run out of drops, stop
                if (RemainingDrops <= 0)
                {
                    BrokeNode = true;
                    break;
                }
                //Otherwise keep checking
                continue;
            }
            //otherwise decrease threshold for the next drop
            _damageTillNextDrop -= toolDamage;
            break;
        }

        //Initialize list of (possibly) random drops
        for (int i = 0; i < fullDrops; i++)
        {
            DropCount.Add(UnityEngine.Random.Range(ItemCountPerDrop.x, ItemCountPerDrop.y));
        }
        return true;
    }

    public ResourceNode(CustomResourceNode Node)
    {
        ToolNeeded = Node.RequiredTool;
        NodeType = Node.NodeType;
        NodePrefabID = Node.NodeDatabaseID;
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





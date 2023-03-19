using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ResourceNode
{


    //Node Characteristics (tier, type, etc)

    [field: SerializeField] public ResourceNodeBase NodeBase { get; private set; }


    //Settings that can be randomized to a unique instance
    [field: SerializeField] public Vector2Int TilePos { get; private set; }

    /// <summary>
    /// Max number of drops a node will provide before breaking
    /// </summary>
    [field: SerializeField] public int MaxDrops { get; private set; }

    /// <summary>
    /// The number of possible drops remanining on the node
    /// </summary>
    [field: SerializeField] public int RemainingDrops { get; private set; }

    #region Helpers
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
    public Guid NodePrefabID => NodeBase.NodePrefabID;
    public NodeType NodeType => NodeBase.NodeType;
    public ToolType ToolNeeded => NodeBase.ToolNeeded;
    public int NodeTier => NodeBase.NodeTier;
    public float TimeToHarvest => NodeBase.TimeToHarvest;
    public int DamagePerDrop => NodeBase.DamagePerDrop;
    Vector2Int ItemCountPerDrop => NodeBase.TableRollsPerPop;
    #endregion


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

[Serializable]
public class ResourceDropTable
{
    [SerializeField] float something;
    System.Random rand = new();
    public ResourceBase GetDrop()
    {
        return null;
    }
}

[Serializable]
public class ItemDropTable<T>
{
    [SerializeField] List<DropTableItem<T>> tableItems;


    public void AddItem(T item)
    {
        tableItems.Add(new DropTableItem<T> { Item = item });
    }
    public T GetDrop()
    {
        System.Random rand = new();
        return tableItems.RandomListItem().Item;
    }
}



[Serializable]
public class DropTableItem<T>
{
    [SerializeReference] public T Item;
    [Range(0, 1)] public float DropChance;
}





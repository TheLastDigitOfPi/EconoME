using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Resource Node Base", menuName = "ScriptableObjects/Tiles/Resource Nodes/Resource Node Base")]
public class ResourceNodeBase : ScriptableObject
{
    /// <summary>
    /// The prefab used to spawn in this node
    /// </summary>
    [field: SerializeField] public GameObject NodePrefab { get; private set; }
    /// <summary>
    /// The ID of the prefab. Used when saving and loading
    /// </summary>
    [field: SerializeField] public Guid NodePrefabID { get; private set; }

    /// <summary>
    /// The tool type required to harvest this resource node. Ex: An Axe to cut a tree, a pickaxe to break a rock, etc
    /// </summary>
    [field: SerializeField] public ToolType ToolNeeded { get; private set; }

    /// <summary>
    /// The tier this node belongs to.
    /// </summary>
    [field: SerializeField] public int NodeTier { get; private set; }

    /// <summary>
    /// The time is takes to use a tool with speed 1 on the node in seconds
    /// </summary>
    [field: SerializeField] public float TimeToHarvest { get; private set; } = 1;

    /// <summary>
    /// The amount of damage needed to be done to the node in order to gain a single drop
    /// </summary>
    [field: SerializeField] public int DamagePerDrop { get; private set; }

    /// <summary>
    /// The range for number of items that will be produced on a single drop
    /// </summary>
    [field: SerializeField] public Vector2Int TableRollsPerPop { get; private set; }

    /// <summary>
    /// The Specific type of node (Tree, rock, etc)
    /// </summary>
    [field: SerializeField] public NodeType NodeType { get; private set; }

    [field:SerializeField] public ItemDropTable<DefinedScriptableItem> ResourceDropTable {get; private set;}
}

public enum NodeType
{
    Tree,
    Rock,
    Bush,
    Plant
}





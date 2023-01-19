using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Resource Node Object", menuName = "ScriptableObjects/TileGeneration/DataObjects/Custom Resource Node")]
[System.Serializable]
public class CustomResourceNode : ScriptableObject
{
    public int NodeDatabaseID;
    public ResourceNodeHandler prefab;
    public ResourceNodeType NodeType;
    public ToolType RequiredTool;

}

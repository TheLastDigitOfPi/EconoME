using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "New Inventory Object", menuName = "ScriptableObjects/Player/Inventory Object")]
public class InventoryObject : ScriptableObject
{
    public SerializableInventory data;
}

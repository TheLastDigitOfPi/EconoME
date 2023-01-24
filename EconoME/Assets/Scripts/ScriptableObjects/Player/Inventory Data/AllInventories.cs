using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "All Inventories For Player", menuName = "ScriptableObjects/Player/Inventory/AllInventories")]
public class AllInventories : ScriptableObject
{
    [field: SerializeField] public List<UnlockableInventory> Inventories { get; private set; }
}


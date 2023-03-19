using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryVariables
{
    [field: SerializeField] public List<BoolVariable> CurrentOpenInventories { get; private set; } = new();
}

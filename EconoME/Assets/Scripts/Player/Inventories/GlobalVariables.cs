using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than 1 Global Variables Singleton Found");
            Destroy(this);
        }
        Instance = this;
    }
    [field: SerializeField] public InventoryVariables InventoryVariables { get; private set; } = new();
    public int Something = 5;
}

[System.Serializable]
public class InventoryVariables
{
    [field: SerializeField] public List<BoolVariable> CurrentOpenInventories { get; private set; } = new();
}

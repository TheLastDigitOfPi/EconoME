using System.Collections.Generic;
using UnityEngine;

public class GlobalLocationHandler : MonoBehaviour
{
    //Static instance
    public static GlobalLocationHandler Instance;

    //public fields
    [field: SerializeField] public List<SceneTransitioner> GlobalTransitions { get; private set; } = new();
    [field: SerializeField] public List<WorldLocationData> WorldLocations { get; private set; } = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 global location handler found!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public static void AddTransitioner(SceneTransitioner transitioner)
    {
        Instance.GlobalTransitions.Add(transitioner);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTileManager : MonoBehaviour
{
    public static WorldTileManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 WorldTileManager Found");
            Destroy(this);
            return;
        }
        Instance = this;
    }
}

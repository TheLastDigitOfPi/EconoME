using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class WorldItemsData
{
    public List<WorldItemInstance> ItemsData;

    public WorldItemsData()
    {
        ItemsData = new List<WorldItemInstance>();
    }
}

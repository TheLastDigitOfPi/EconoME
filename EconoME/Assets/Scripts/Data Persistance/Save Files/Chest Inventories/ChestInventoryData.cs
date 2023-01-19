using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ChestInventoryData
{
    public List<ChestInstanceData> data = new List<ChestInstanceData>();

    public ChestInventoryData()
    {
        data = new List<ChestInstanceData>();
    }
}

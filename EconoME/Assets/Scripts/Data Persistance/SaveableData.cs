using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveableData
{
    public List<IntVariable> IntVariables = new();
    public List<FloatVariable> FloatVariables = new();
    public List<BoolVariable> BoolValues = new();
    public List<Vector3Variable> Vector3Variables = new();
}

[Serializable]
public class SerializableSavableData
{
    public List<DataContainer<int>> IntData = new();
    public List<DataContainer<float>> FloatData = new();
    public List<DataContainer<bool>> BoolData = new();
    public List<DataContainer<Vector3>> Vector3Data = new();

    public SerializableSavableData(SaveableData data)
    {

        for (int i = 0; i < data.IntVariables.Count; i++)
        {
            IntData.Add(new DataContainer<int>(data.IntVariables[i].ValueName, data.IntVariables[i].Value));
        }

        for (int i = 0; i < data.FloatVariables.Count; i++)
        {
            FloatData.Add(new DataContainer<float>(data.FloatVariables[i].ValueName, data.FloatVariables[i].Value));
        }

        for (int i = 0; i < data.BoolValues.Count; i++)
        {
            BoolData.Add(new DataContainer<bool>(data.BoolValues[i].ValueName, data.BoolValues[i].Value));
        }

        for (int i = 0; i < data.Vector3Variables.Count; i++)
        {
            Vector3Data.Add(new DataContainer<Vector3>(data.Vector3Variables[i].ValueName, data.Vector3Variables[i].Value));
        }

    }
}

using System;

[Serializable]
public class DataContainer<T>
{
    public T Value;
    public string ValueName;

    public DataContainer(string Name, T value)
    {
        ValueName = Name;
        Value = value;
    }
}





public interface ISaveableObject
{
    string ValueName { get; set; }
    string Save();
    bool Load<T>(DataContainer<T> data);
}




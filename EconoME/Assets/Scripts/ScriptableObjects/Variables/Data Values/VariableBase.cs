using System;
using UnityEngine;

public abstract class VariableBase<T> : ScriptableObject
{
    [Header("Defalts")]
    [SerializeField] T DefaultValue;
    [SerializeField] bool ResetToDefault;
    [Space(10)]
    [Header("Values")]
    public string ValueName;
    [SerializeField] T _value;
    public T Value { get { return _value; } set { _value = value; onValueChange?.Invoke(); } }
    public Action onValueChange;
    public string Save()
    {
        return JsonUtility.ToJson(this);
    }
    public bool Load(VariableBase<T> data)
    {
        if (data.ValueName != ValueName) { return false; }
        Value = data.Value;
        return true;
    }
    public void OnValidate()
    {
        ValueName = name;
    }

    private void OnDisable()
    {
        if (ResetToDefault)
        {
            Value = DefaultValue;
            onValueChange = null;
        }
    }

    private void OnEnable()
    {
        if (ResetToDefault)
        {
            Value = DefaultValue;
            onValueChange = null;
        }
    }
}

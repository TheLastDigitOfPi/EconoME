using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ModifiableStat<T>
{
    [field:SerializeReference] public T BaseValue { get; protected set; }
    [field:SerializeReference] public T CurrentValue { get; protected set; }
    [SerializeField] protected List<StatChanger> changes = new();
    public virtual void AddChange(StatChanger change)
    {
        changes.Add(change);
        RecalculateStat();
    }

    public virtual void RemoveChange(StatChanger change)
    {
        changes.Remove(change);
        RecalculateStat();
    }
    abstract protected void RecalculateStat();
}

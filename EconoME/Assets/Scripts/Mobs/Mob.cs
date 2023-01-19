using System;
using UnityEngine;

public abstract class Mob : MonoBehaviour
{
    [field: SerializeField] public int Health { get; protected set; }
    [field: SerializeField] public bool IsScared { get; protected set; }
    [field: SerializeField] public bool IsHappy { get; protected set; }
    [field: SerializeField] public bool IsAngry { get; protected set; }
    [field: SerializeField] public bool IsNeutral { get; protected set; } = true;
    public bool IsAlive { get{return Health > 0;} protected set{ } }

    [field: SerializeField] public float Speed { get; internal set; }

    public virtual void OnDeath()
    {
        Debug.Log("Mob is dead");
    }

    internal virtual void LostPlayer()
    {
        IsAngry = false;
        IsNeutral = true;
    }
}

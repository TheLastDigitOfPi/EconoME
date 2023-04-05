using UnityEngine;

public abstract class EventTriggerSO : ScriptableObject
{
    public abstract EventTrigger GetTrigger();
}

/// <summary>
/// This class tells us how we will know when the trigger happens, usually by subscribing to an event
/// </summary>
public abstract class EventTrigger
{
    public abstract void AddEffect(ArmorEffect effect, EntityCombatController owner);
    public abstract void UnloadEvent(ArmorEffect effect, EntityCombatController owner);
}
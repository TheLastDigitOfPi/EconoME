using UnityEngine;

public abstract class ArmorEffectSO : ScriptableObject
{
    public abstract bool TryGetEffect(EntityCombatController owner, out ArmorEffect effect);
}

[System.Serializable]
public abstract class ArmorEffect
{
    public abstract void TriggerEffect();
    public abstract void OnUnequip();
}
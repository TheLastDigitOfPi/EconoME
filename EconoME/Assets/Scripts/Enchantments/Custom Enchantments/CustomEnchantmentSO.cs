using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObjects/Enchants/Custom/New Custom Enchantment", fileName = "New Custom Enchantment")]
public class CustomEnchantmentSO : ArmorEnchantmentSO
{
    [SerializeField] EventTriggerSO TriggerSO;
    [SerializeField] ArmorEffectSO[] EffectsSO;

    public override bool TryGetEnchantment(EntityCombatController owner, out ArmorEnchantment enchantment)
    {
        enchantment = new CustomEnchantment(TriggerSO, EffectsSO, owner);
        return enchantment != null;
    }
}

[System.Serializable]
public class CustomEnchantment : ArmorEnchantment
{
    private EventTrigger trigger;
    [SerializeReference] ArmorEffect[] effects;

    public CustomEnchantment(EventTriggerSO triggerSO, ArmorEffectSO[] effectSO, EntityCombatController owner)
    {
        trigger = triggerSO.GetTrigger();
        List<ArmorEffect> validEffects = new();
        foreach (var effect in effectSO)
        {
            if(effect.TryGetEffect(owner, out var effectEffect))
                validEffects.Add(effectEffect);
        }
        effects = validEffects.ToArray();
    }

    public override void OnEquip(EntityCombatController owner)
    {
        base.OnEquip(owner);
        if (effects == null)
            return;
        foreach (var effect in effects)
        {
            trigger.AddEffect(effect, owner);
        }
    }

    public override void OnUnequip()
    {
        if (effects == null)
            return;
        foreach (var effect in effects)
        {
            trigger.UnloadEvent(effect, owner);
            effect.OnUnequip();
        }
    }
}
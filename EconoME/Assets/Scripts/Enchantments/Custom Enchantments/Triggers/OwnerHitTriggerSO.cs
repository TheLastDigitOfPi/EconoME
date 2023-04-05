using UnityEngine;
public class OwnerHitTrigger : EventTrigger
{
    public override void AddEffect(ArmorEffect effect, EntityCombatController owner)
    {
        owner.OnDefense += effect.TriggerEffect;
    }

    public override void UnloadEvent(ArmorEffect effect, EntityCombatController owner)
    {
        owner.OnDefense -= effect.TriggerEffect;
    }
}

[CreateAssetMenu(fileName = "Owner Hit Trigger", menuName = "ScriptableObjects/Enchants/Custom/Triggers/Owner Hit")]
public class OwnerHitTriggerSO : EventTriggerSO
{
    public override EventTrigger GetTrigger()
    {
        return new OwnerHitTrigger();
    }
}


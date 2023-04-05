using UnityEngine;

[CreateAssetMenu(fileName = "Owner Game Tick Trigger", menuName = "ScriptableObjects/Enchants/Custom/Triggers/Game Tick")]
public class GameTickTriggerSO : EventTriggerSO
{
    public override EventTrigger GetTrigger()
    {
        return new GameTickTrigger();
    }
}

public class GameTickTrigger : EventTrigger
{
    public override void AddEffect(ArmorEffect effect, EntityCombatController owner)
    {
        WorldTimeManager.OnGameTick += effect.TriggerEffect;  
    }

    public override void UnloadEvent(ArmorEffect effect, EntityCombatController owner)
    {
        WorldTimeManager.OnGameTick -= effect.TriggerEffect;  
    }
}

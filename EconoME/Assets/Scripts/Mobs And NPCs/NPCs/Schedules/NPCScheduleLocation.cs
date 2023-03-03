using UnityEngine;


[CreateAssetMenu(fileName = "New NPC Schedule - Location", menuName = "ScriptableObjects/NPCs/Schedules/Schedule Items/Location")]
public class NPCScheduleLocation : NPCScheduleItem
{
    [field: SerializeField] public WorldLocationData Location { get; private set; }

    public override bool OnScheduleInterrupt(NPCScheduleManager manager)
    {
        throw new System.NotImplementedException();
    }

    public override bool OnScheduleStart(NPCScheduleManager manager)
    {
        manager.Npc.TravelingManager.GoToLocation(Location);
        return true;
    }
}

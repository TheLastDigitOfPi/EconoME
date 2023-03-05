using UnityEngine;


[CreateAssetMenu(fileName = "New NPC Schedule - Location", menuName = "ScriptableObjects/NPCs/Schedules/Schedule Items/Location")]
public class NPCScheduleLocation : NPCScheduleItem
{
    [field: SerializeField] public WorldLocationData Location { get; private set; }

    public override bool OnScheduleInterrupt(NPCScheduleHandler manager)
    {
        manager.Npc.TravelingManager.InterruptTravel();
        return true;
    }

    public override bool OnScheduleStart(NPCScheduleHandler manager)
    {
        Debug.Log("Started Location schedule");
        manager.Npc.TravelingManager.GoToLocation(Location);
        return true;
    }

    public override bool OnScheduleStop(NPCScheduleHandler manager)
    {
        Debug.Log("Left Location schedule");
        manager.Npc.TravelingManager.StopTravel();
        return true;
    }

    public override bool OnScheduleTick(NPCScheduleHandler manager)
    {
        manager.Npc.TravelingManager.TravelTickCheck();
        Debug.Log("Location Schedule Tick");
        return true;
    }
}

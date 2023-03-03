using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Schedule - Work", menuName = "ScriptableObjects/NPCs/Schedules/Schedule Items/Work")]
public class NPCScheduleWork : NPCScheduleLocation
{
    //Settings

    [field: SerializeField] public string NPCWorkAnimationName {get; private set;} = "Work";

    //Local fields
    NPC _npc;
    NPCTravelingManager _travelingManger;
    Animator _animator;
    public override bool OnScheduleStart(NPCScheduleManager manager)
    {
        _npc = manager.Npc;
        _travelingManger = manager.Npc.TravelingManager;
        _travelingManger.GoToLocation(Location);
        _travelingManger.OnReachDestination += StartWorking;
        return true;
    }

    private void StartWorking()
    {
    }
}

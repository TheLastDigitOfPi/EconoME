using UnityEngine;

public class NPCScheduleManager : MonoBehaviour
{
    [SerializeField] NPCSchedule schedule;
    public WorldLocation location;
    public NPC Npc { get; private set; }

    private void Start()
    {
        WorldTimeManager.Instance.OnGameTick += SearchForSchedule;
        Npc = GetComponent<NPC>();
    }

    private void SearchForSchedule()
    {
        //Serach the current schedule for if we need to start doing a new action
    }

    private void OnDestroy()
    {
        WorldTimeManager.Instance.OnGameTick -= SearchForSchedule;
    }

    public bool GetTravelLocation(out WorldLocationData location)
    {
        return schedule.GetTravelLocation(out location);
    }

}

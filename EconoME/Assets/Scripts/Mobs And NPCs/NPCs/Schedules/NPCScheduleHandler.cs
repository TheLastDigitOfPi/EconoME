using System;
using UnityEngine;

public class NPCScheduleHandler : MonoBehaviour
{

    //Public fields
    public NPC Npc { get; private set; }
    [field: SerializeField] public bool CheckingSchedule { get; private set; } = false;
    [field: SerializeField] public bool InFreeTime { get; private set; } = false;
    //Local fields
    [SerializeField] NPCSchedule schedule;
    [SerializeField] NPCScheduleItem currentAction;

    private void Start()
    {
        WorldTimeManager.OnGameTick += SearchForSchedule;
        Npc = GetComponent<NPC>();
    }

    private void SearchForSchedule()
    {
        //Serach the current schedule for if we need to start doing a new action
        var time = WorldTimeManager.CurrentTime;

        //Only check every other tick
        if(time.TimeOfDayTick % 2 != 0)
            return;

        InFreeTime = !EnsureActiveAndValidSchedule();

        bool EnsureActiveAndValidSchedule()
        {
            //Check if override needs to happen
            var overrideItems = schedule.TodaysOverrideItems;

            foreach (var item in overrideItems)
            {
                //If we found a valid time, set it to our current schedule item
                if (time.TimeOfDayTick.isBetweenInclusive(item.StartTime, item.EndTime))
                {
                    currentAction.OnScheduleInterrupt(this);
                    currentAction = item;
                    currentAction.OnScheduleStart(this);
                    return true;
                }
            }
            //If our current action exists, check if it is still valid
            if (currentAction != null)
            {
                if (time.TimeOfDayTick.isBetweenInclusive(currentAction.StartTime, currentAction.EndTime))
                    return true;

                currentAction.OnScheduleStop(this);
                currentAction = null;
            }

            //If we don't have a current schedule item, try to find one
            if (currentAction == null)
            {
                var todayItems = schedule.TodaysScheduleItems;
                //Check normal schedule for items
                foreach (var scheduleItem in todayItems)
                {
                    //If we found a valid time, set it to our current schedule item
                    if (time.TimeOfDayTick.isBetweenInclusive(scheduleItem.StartTime, scheduleItem.EndTime))
                    {
                        currentAction = scheduleItem;
                        currentAction.OnScheduleStart(this);
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

    }

    private void OnDestroy()
    {
        WorldTimeManager.OnGameTick -= SearchForSchedule;
    }

    public bool GetTravelLocation(out WorldLocationData location)
    {
        return schedule.GetTravelLocation(out location);
    }

    internal void ScheduleTick()
    {
        //Perform current schedule item
        currentAction?.OnScheduleTick(this);
    }

    internal void StopSchedule()
    {
        Debug.Log("NPC should now no longer being following the schedule");
    }

    internal void ResumeSchedule()
    {
        Debug.Log("NPC should now be following the schedule");
    }
}

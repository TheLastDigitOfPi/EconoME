using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New NPC Scheduler", menuName = "ScriptableObjects/NPCs/Schedules/Scheduler")]
public class NPCSchedule : ScriptableObject
{
    [field: SerializeReference] public List<NPCScheduleItem> SundaySchedule { get; private set; }
    [field: SerializeReference] public List<NPCScheduleItem> MondaySchedule { get; private set; }
    [field: SerializeReference] public List<NPCScheduleItem> TuesdaySchedule { get; private set; }
    [field: SerializeReference] public List<NPCScheduleItem> WednesdaySchedule { get; private set; }
    [field: SerializeReference] public List<NPCScheduleItem> ThursdaySchedule { get; private set; }
    [field: SerializeReference] public List<NPCScheduleItem> FridaySchedule { get; private set; }
    [field: SerializeReference] public List<NPCScheduleItem> SaturdaySchedule { get; private set; }
    [field: SerializeReference] public List<NPCScheduleItem> ScheduleOverrides { get; private set; }

    [SerializeReference] List<NPCScheduleItem> InvalidScheduleItems;
    public void SortList()
    {
        //Sort by time
        SundaySchedule.Sort((s1, s2) => s1.StartTime.CompareTo(s2.StartTime));
        MondaySchedule.Sort((s1, s2) => s1.StartTime.CompareTo(s2.StartTime));
        TuesdaySchedule.Sort((s1, s2) => s1.StartTime.CompareTo(s2.StartTime));
        WednesdaySchedule.Sort((s1, s2) => s1.StartTime.CompareTo(s2.StartTime));
        ThursdaySchedule.Sort((s1, s2) => s1.StartTime.CompareTo(s2.StartTime));
        FridaySchedule.Sort((s1, s2) => s1.StartTime.CompareTo(s2.StartTime));
        SaturdaySchedule.Sort((s1, s2) => s1.StartTime.CompareTo(s2.StartTime));
    }
    public void ValidateSchedules()
    {

        ValidateSchedule(SundaySchedule);
        ValidateSchedule(MondaySchedule);
        ValidateSchedule(TuesdaySchedule);
        ValidateSchedule(WednesdaySchedule);
        ValidateSchedule(ThursdaySchedule);
        ValidateSchedule(FridaySchedule);
        ValidateSchedule(SaturdaySchedule);

        void ValidateSchedule(List<NPCScheduleItem> Schedule)
        {
            //Copy list to temporay list
            List<NPCScheduleItem> ItemsForDay = new();
            foreach (var item in Schedule)
            {
                ItemsForDay.Add(item);
            }

            //Go through each item for the day

            for (int i = 0; i < ItemsForDay.Count; i++)
            {
                var item = ItemsForDay[i];
                for (int j = 0; j < ItemsForDay.Count; j++)
                {
                    var otherItem = ItemsForDay[j];
                    //Don't check the same item
                    if (otherItem == item)
                        continue;


                    /* IV = Invalid, Time is item we are checking, V = Valid
                        |  IV  ||  IV  |
                          |   TIME   |
                             |  IV  |
                          |     IV      |
                                       |    V    |
                     */

                    //If the start time is at or between this time, invalid
                    bool startTimeConflict = otherItem.StartTime.isBetweenInclusive(item.StartTime, item.EndTime);
                    //If the end time is at or between this time, invalid
                    bool endTimeConflict = otherItem.EndTime.isBetweenInclusive(item.StartTime, item.EndTime);

                    //If the other items have a start time or end time that conflicts, remove it
                    if (startTimeConflict || endTimeConflict)
                    {
                        Debug.LogWarning("Scheudle conflic of " + item.name + " and " + otherItem.name + " removing " + otherItem.name + " from schedule");
                        InvalidScheduleItems.Add(otherItem);
                        ItemsForDay.RemoveAt(j);
                        Schedule.Remove(otherItem);
                        j--;
                    }
                }
                ItemsForDay.RemoveAt(i);
                i--;
            }
            foreach (var item in ItemsForDay)
            {
                bool itemValid = true;
                //Check all the other items with this one
                foreach (var otherItem in ItemsForDay)
                {

                }


            }
        }


    }

    public List<NPCScheduleItem> TodaysOverrideItems
    {
        get
        {
            int day = WorldTimeManager.CurrentTime.Day % 7;
            List<NPCScheduleItem> items = new();
            foreach (var item in ScheduleOverrides)
            {
                if (item.DayOfWeek == day)
                {
                    items.Add(item);
                }
            }
            return items;
        }
    }

    public List<NPCScheduleItem> TodaysScheduleItems
    {
        get
        {
            int day = WorldTimeManager.CurrentTime.Day % 7;
            List<NPCScheduleItem> items;
            switch (day)
            {
                case 0:
                    items = SundaySchedule;
                    break;
                case 1:
                    items = MondaySchedule;
                    break;
                case 2:
                    items = TuesdaySchedule;
                    break;
                case 3:
                    items = WednesdaySchedule;
                    break;
                case 4:
                    items = ThursdaySchedule;
                    break;
                case 5:
                    items = FridaySchedule;
                    break;
                case 6:
                    items = SaturdaySchedule;
                    break;
                default:
                    items = new();
                    break;
            }
            return items;
        }
    }

    public bool GetTravelLocation(out WorldLocationData location)
    {
        location = null;
        List<NPCScheduleItem> CurrentDaySchedule;
        switch (WorldTimeManager.CurrentTime.Day)
        {
            case 0:
                CurrentDaySchedule = SundaySchedule;
                break;
            case 1:
                CurrentDaySchedule = SundaySchedule;
                break;
            case 2:
                CurrentDaySchedule = SundaySchedule;
                break;
            case 3:
                CurrentDaySchedule = SundaySchedule;
                break;
            case 4:
                CurrentDaySchedule = SundaySchedule;
                break;
            case 5:
                CurrentDaySchedule = SundaySchedule;
                break;
            case 6:
                CurrentDaySchedule = SundaySchedule;
                break;
            default:
                CurrentDaySchedule = SundaySchedule;
                break;
        }

        foreach (var item in CurrentDaySchedule)
        {
            if (WorldTimeManager.CurrentTime.TimeOfDayTick.isBetweenInclusive(item.StartTime, item.EndTime))
            {
                if (item is not NPCScheduleLocation)
                    return false;
                location = (item as NPCScheduleLocation).Location;
            }
        }
        return false;
    }
}

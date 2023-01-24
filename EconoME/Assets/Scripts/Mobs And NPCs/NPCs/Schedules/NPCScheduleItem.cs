using UnityEngine;

/// <summary>
/// Represents an action that will be taken during the NPC's schedule. Schedule items must not overlap, but overrides are avaialble.
/// </summary> 
/// 

/*

This will be used to perform certain actions throughout the day. 

Possible Examples

-going to a certain location, then chilling
-sleeping
-wandering 
-going to work
-going to church
-going to the fair
-hunting
-mining
-going to the bank
-going home
Basically anything that a person might have on their schedule. Think in a way like "what am I going to go do now?"

Consider also making an NPC schedule item that is random. The NPC may randomly choose between a few schedule items
For example
NPC entered "Free Time" (Npc random schedule). They may choose to either go Home, go to the Store, the park, etc.
 
 */
public abstract class NPCScheduleItem : ScriptableObject
{
    public virtual int StartTime { get { return _startTime.StandardizedTime; } }
    public virtual int EndTime { get { return _endTime.StandardizedTime; } }
    [field: SerializeField] public int DayOfWeek { get; private set; }
    [SerializeField] GameTime _startTime;
    [SerializeField] GameTime _endTime;
    public abstract bool OnScheduleStart(NPCScheduleManager manager);
    public abstract bool OnScheduleInterrupt(NPCScheduleManager manager);
}

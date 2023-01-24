using UnityEngine;

[CreateAssetMenu(fileName = "New World Time", menuName = "ScriptableObjects/Events/Time/World Time")]
public class TimeObject : ScriptableObject
{
    public int Day;
    public int Week;
    public int Month;
    public int Year;

    //Settings
    [field: SerializeField] public int MaxTicksInDay { get; private set; } = 24000;
    [field: SerializeField] public int TicksPerSecond { get; private set; } = 5;
    [field: SerializeField] public int WeeksInMonth { get; private set; } = 4;

    [SerializeField] int TotalSecondsPerDay;

    

    public int TicksPerDay { get { return MaxTicksInDay / TicksPerSecond; } }
    public int TimeOfDayTick { get; set; }

    public void UpdateTotalSecondsPerDay()
    {
        TotalSecondsPerDay = MaxTicksInDay / TicksPerSecond;
    }
}



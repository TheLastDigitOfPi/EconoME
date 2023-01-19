using UnityEngine;

[CreateAssetMenu(fileName = "New World Time", menuName = "ScriptableObjects/Events/Time/World Time")]
public class TimeObject : ScriptableObject
{
    public float TimeOfDay;
    public int Day;
    public int Week;
    public int Month;
    public int Year;

    public int MaxTicksInDay = 24000;
    public int TicksPerSecond = 5;
    public int WeeksInMonth = 0;

    [Range(0, 1)] public float DawnStart;
    [Range(0, 1)] public float DuskStart;
    public int SunTransitionInSeconds;
    [Range(0, 1)] public float LightMax;
    [Range(0, 1)] public float LightMin;
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/*
 * Controls the time state of the game
 * 
 * 
 */
public class WorldTimeManager : MonoBehaviour
{
    //Static Data
    public static WorldTimeManager Instance;
    public static TimeObject CurrentTime { get { return Instance.WorldTime; } }
    public static int HumanTimeToGameTime(int hour, int minute)
    {
        /*
            Human world time has a total of 1440 (24 hours * 60 minutes) 'ticks' per day
            To convert, simply find the percent of day completed in normal hours and apply that to the tick standard
        */

        //Get the percentage of day we have complete
        int humanTimeTick = (hour * 60) + minute;
        float humanTimeTickPercent = humanTimeTick / 1440f;

        //Return default time scale in case SO cannot access instance due to game not started
        if (Instance == null)
        {
            return (int)(24000 * humanTimeTickPercent);
        }

        //return the tick at that percentage
        return (int)(Instance.WorldTime.MaxTicksInDay * humanTimeTickPercent);
    }

    public static string CurrentGameTimeToHumanTime
    {
        get
        {
            var percent = (float)CurrentTime.TimeOfDayTick / CurrentTime.MaxTicksInDay;

            var currentClockTicks = percent * 1440f;
            return $"{(int)(percent * 24)} : {(int)currentClockTicks % 60}";
        }
    }


    public static event Action OnGameTick { add { Instance._onGameTick += value; } remove { Instance._onGameTick -= value; } }
    public static event Action OnNewDay { add { Instance._onNewDay += value; } remove { Instance._onNewDay -= value; } }
    public static event Action OnNewWeek { add { Instance._onNewWeek += value; } remove { Instance._onNewWeek -= value; } }
    public static event Action OnNewMonth { add { Instance._onNewMonth += value; } remove { Instance._onNewMonth -= value; } }

    [SerializeField] TimeObject WorldTime;

    //Public events

    event Action _onGameTick;
    event Action _onNewDay;
    event Action _onNewWeek;
    event Action _onNewMonth;


    //Public fields
    public bool ClockRunning { get { return Application.isPlaying && !TimePaused; } }

    //Local helpers
    int Day { get { return WorldTime.Day; } set { WorldTime.Day = value; } }
    int Week { get { return WorldTime.Week; } set { WorldTime.Week = value; } }
    int Month { get { return WorldTime.Month; } set { WorldTime.Month = value; } }
    int Year { get { return WorldTime.Year; } set { WorldTime.Year = value; } }
    int TicksPerSecond { get { return WorldTime.TicksPerSecond; } }
    int MaxTicksInDay { get { return WorldTime.MaxTicksInDay; } }
    int WeeksInMonth { get { return WorldTime.WeeksInMonth; } }

    //Local fields
    Stopwatch watch;
    [SerializeField] bool ResetTimeOnStart;
    [SerializeField] GameTime ResetTime;
    public bool TimePaused;

    Coroutine currentRunningClock;

    private void Awake()
    {
        if (Instance != null)
        {
            UnityEngine.Debug.LogWarning("Attempted to make more than 1 WorldTimeHandler Instance");
            Destroy(this);
        }
        Instance = this;

        if (ResetTimeOnStart)
        {
            CurrentTime.ResetTime(ResetTime);
        }

    }


    private void Start()
    {
        currentRunningClock = StartCoroutine(ClockTick());
    }

    private void OnDestroy()
    {
    }

    private void NewMonth()
    {
        Month++;
        _onNewMonth?.Invoke();
    }

    private void NewWeek()
    {
        Week++;
        _onNewWeek?.Invoke();
        if (Week >= WeeksInMonth)
            NewMonth();
    }

    private void NewDay()
    {
        Day++;
        WorldTime.TimeOfDayTick = 0;
        _onNewDay?.Invoke();

        if (Day >= 7)
            NewWeek();
    }

    WaitForFixedUpdate waitForFixedUpdate = new();
    float timeHolder = 0;
    IEnumerator ClockTick()
    {
        do
        {
            if (!ClockRunning)
            {
                yield return new WaitUntil(() => ClockRunning);
            }

            timeHolder += Time.deltaTime;

            int secondsThisFrame = (int)timeHolder;
            int ticksThisFrame = (int)(timeHolder * WorldTime.TicksPerSecond);
            float ticksPerSec = 1f / WorldTime.TicksPerSecond;
            if (ticksThisFrame >= 1)
            {
                timeHolder -= ticksPerSec * ticksThisFrame;
                WorldTime.TimeOfDayTick += (ticksThisFrame);
                UpdateClock();
            }
            //Wait for the next tick
            yield return waitForFixedUpdate;
        } while (true);

    }

    void UpdateClock()
    {
        _onGameTick?.Invoke();
        if (WorldTime.TimeOfDayTick >= MaxTicksInDay)
        {
            NewDay();
        }
    }

}

[Serializable]
public struct GameTime
{
    public Vector2Int humanTime;
    public int StandardizedTime { get { return Validate(); } }
    public int Validate()
    {
        var hour = humanTime.x;
        var minute = humanTime.y;

        if (!hour.isBetweenInclusive(0, 23))
        {
            hour = hour > 23 ? 23 : 0;
        }
        if (!minute.isBetweenInclusive(0, 59))
        {
            minute = minute > 59 ? 59 : 0;
        }

        return WorldTimeManager.HumanTimeToGameTime(hour, minute);

    }
}
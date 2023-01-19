using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/*
 * Controls the time state of the game
 * 
 * 
 */
public class WorldTimeHandler : MonoBehaviour
{
    public static WorldTimeHandler Instace;
    [SerializeField] TimeObject WorldTime;

    float TimeInDay { get { return WorldTime.TimeOfDay; } set { WorldTime.TimeOfDay = value; } }
    int Day { get { return WorldTime.Day; } set { WorldTime.Day = value; } }
    int Week { get { return WorldTime.Week; } set { WorldTime.Week = value; } }
    int Month { get { return WorldTime.Month; } set { WorldTime.Month = value; } }
    int Year { get { return WorldTime.Year; } set { WorldTime.Year = value; } }
    int TicksPerSecond { get { return WorldTime.TicksPerSecond; } }
    int MaxTicksInDay { get { return WorldTime.MaxTicksInDay; } }
    int WeeksInMonth { get { return WorldTime.WeeksInMonth; } }
    float DawnStart { get { return WorldTime.DawnStart; } }
    float DuskStart { get { return WorldTime.DuskStart; } }
    int SunTransitionInSeconds { get { return WorldTime.SunTransitionInSeconds; } }
    float LightMax { get { return WorldTime.LightMax; } }
    float LightMin { get { return WorldTime.LightMin; } }

    bool SunRise;
    bool SunFall;
    Stopwatch watch;
    [SerializeField] Light2D GlobalLight;

    public bool TimePaused;

    private void Awake()
    {
        if (Instace != null)
        {
            UnityEngine.Debug.LogWarning("Attempted to make more than 1 WorldTimeHandler Instance");
            Destroy(this.gameObject);
        }
        Instace = this;
        watch = new Stopwatch();
    }

    private void Update()
    {
        if (!Application.isPlaying) { return; }
        if(WorldTime == null){return;}

        TimeInDay += TicksPerSecond * Time.deltaTime;

        if (TimeInDay / MaxTicksInDay > DawnStart && !SunRise)
        {
            SunRise = true;
            watch.Start();
            StartCoroutine(SunTransition(true));

        }

        if (TimeInDay / MaxTicksInDay > DuskStart && !SunFall)
        {
            SunFall = true;
            StartCoroutine(SunTransition(false));
        }

        if (TimeInDay >= MaxTicksInDay)
        {
            Day++;
            if (Day >= 7)
            {
                Day = 0;
                Week++;
            }
            if (Week >= WeeksInMonth)
            {
                Month++;
                Week = 0;
            }
            TimeInDay = 0;
            SunRise = false;
            SunFall = false;
        }

    }

    IEnumerator SunTransition(bool Rising)
    {
        if (Rising)
        {
            while (GlobalLight.intensity <= LightMax)
            {
                GlobalLight.intensity += ((LightMax - LightMin) / (SunTransitionInSeconds) * Time.deltaTime);
                yield return null;
            }
            GlobalLight.intensity = LightMax;
            watch.Stop();
            UnityEngine.Debug.Log("Timer was :" + watch.Elapsed.ToString());
            watch.Reset();
        }
        else
        {
            while (GlobalLight.intensity >= LightMin)
            {
                GlobalLight.intensity -= ((LightMax - LightMin) / (SunTransitionInSeconds) * Time.deltaTime);
                yield return null;
            }
            GlobalLight.intensity = LightMin;
        }

    }


}

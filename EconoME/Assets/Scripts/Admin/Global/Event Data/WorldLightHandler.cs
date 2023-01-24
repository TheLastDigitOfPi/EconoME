using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldLightHandler : MonoBehaviour
{
    //Public events
    public event Action OnSunRise;
    public event Action OnSunSet;

    //Public fields
    [field: SerializeField] public int SunTransitionTimeInSeconds { get; private set; }

    [field: SerializeField] GameTime SunRiseTime { get; set; }
    [field: SerializeField] GameTime SunSetTime { get; set; }

    [field: SerializeField, Range(0, 1)] float MaxSunBrightness;
    [field: SerializeField, Range(0, 1)] float MinSunBrightness;

    //Local fields
    [SerializeField] Light2D _globalLight;
    bool _sunHasRisen;
    bool _sunHasSet;

    private void Start()
    {
        WorldTimeManager.Instance.OnGameTick += CheckTime;
        WorldTimeManager.Instance.OnNewDay += NewDay;
    }

    private void NewDay()
    {
        _sunHasSet = false;
        _sunHasRisen = false;
    }

    private void CheckTime()
    {
        if (!_sunHasRisen)
        {
            if (WorldTimeManager.CurrentTime.TimeOfDayTick >= SunRiseTime.StandardizedTime)
                StartCoroutine(SunRise());
            return;
        }

        if (!_sunHasSet)
        {
            if (WorldTimeManager.CurrentTime.TimeOfDayTick >= SunSetTime.StandardizedTime)
                StartCoroutine(SunSet());

            return;
        }
    }

    IEnumerator SunRise()
    {
        float brightnessDifference = (MaxSunBrightness - MinSunBrightness);
        float timeRising = 0;
        while (_globalLight.intensity <= MaxSunBrightness)
        {
            //Keep track of time
            timeRising += Time.deltaTime;
            //Get the % we are through the rise starting from 0% to 100%
            float percentThroughRise = timeRising / SunTransitionTimeInSeconds;
            float currentBrightness = (brightnessDifference * percentThroughRise) + MinSunBrightness;
            _globalLight.intensity = currentBrightness;
            yield return null;
        }
        _globalLight.intensity = MaxSunBrightness;
    }

    IEnumerator SunSet()
    {
        float brightnessDifference = (MaxSunBrightness - MinSunBrightness);
        float timeSetting = 0;
        while (_globalLight.intensity <= MaxSunBrightness)
        {
            //Keep track of time
            timeSetting += Time.deltaTime;
            //Get the % we are through the rise starting from 100% to 0%
            float percentThroughRise = 1 - (timeSetting / SunTransitionTimeInSeconds);
            float currentBrightness = (brightnessDifference * percentThroughRise) + MinSunBrightness;
            _globalLight.intensity = currentBrightness;
            yield return null;
        }
        _globalLight.intensity = MinSunBrightness;
    }


}

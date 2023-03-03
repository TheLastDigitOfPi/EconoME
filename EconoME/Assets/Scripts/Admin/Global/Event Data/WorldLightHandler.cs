using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WorldLightHandler : MonoBehaviour
{
    //Static instance
    public static WorldLightHandler Instance { get; private set; }

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
    [SerializeField] Color _nightTimeColor;
    [SerializeField] Color _dayTimeColor;

    bool _sunHasRisen;
    bool _sunHasSet;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 World Light handler found");
            Destroy(this);
            return;
        }
        Instance = this;
    }


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
        if (!_sunHasRisen && !_sunTransitioning)
        {
            if (WorldTimeManager.CurrentTime.TimeOfDayTick >= SunRiseTime.StandardizedTime)
            {
                _sunTransitioning = true;
                StartCoroutine(SunRise());
            }
            return;
        }

        if (!_sunHasSet && !_sunTransitioning)
        {
            if (WorldTimeManager.CurrentTime.TimeOfDayTick >= SunSetTime.StandardizedTime)
            {
                _sunTransitioning = true;
                StartCoroutine(SunSet());
            }
            return;
        }
    }

    bool _sunTransitioning;
    IEnumerator SunRise()
    {
        float brightnessDifference = (MaxSunBrightness - MinSunBrightness);
        float timeRising = 0;
        while (_globalLight.intensity < MaxSunBrightness)
        {
            //Keep track of time
            timeRising += Time.deltaTime;
            //Get the % we are through the rise starting from 0% to 100%
            float percentThroughRise = timeRising / SunTransitionTimeInSeconds;
            float currentBrightness = Math.Clamp((brightnessDifference * percentThroughRise) + MinSunBrightness, MinSunBrightness, MaxSunBrightness);
            _globalLight.intensity = currentBrightness;
            _globalLight.color = Color.Lerp(_nightTimeColor, _dayTimeColor, percentThroughRise);
            yield return null;
        }
        _sunTransitioning = false;
        _sunHasRisen = true;
        OnSunRise?.Invoke();
        _globalLight.intensity = MaxSunBrightness;
        _globalLight.color = _dayTimeColor;
    }

    IEnumerator SunSet()
    {
        float brightnessDifference = (MaxSunBrightness - MinSunBrightness);
        float timeSetting = 0;
        while (_globalLight.intensity > MinSunBrightness)
        {
            //Keep track of time
            timeSetting += Time.deltaTime;
            //Get the % we are through the rise starting from 100% to 0%
            float percentThroughRise = 1 - (timeSetting / SunTransitionTimeInSeconds);
            float currentBrightness = Math.Clamp((brightnessDifference * percentThroughRise) + MinSunBrightness, MinSunBrightness, MaxSunBrightness);
            _globalLight.intensity = currentBrightness;
            _globalLight.color = Color.Lerp(_nightTimeColor, _dayTimeColor, percentThroughRise);
            yield return null;
        }
        _sunTransitioning = false;
        _sunHasSet = true;
        OnSunSet?.Invoke();
        _globalLight.intensity = MinSunBrightness;
        _globalLight.color = _nightTimeColor;
    }


}

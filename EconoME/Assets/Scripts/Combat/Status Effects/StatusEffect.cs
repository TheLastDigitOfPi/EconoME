using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public abstract class StatusEffect : IEquatable<StatusEffect>, IEqualityComparer<StatusEffect>
{
    public event Action OnEffectTimerEnd;
    public int Intensity;
    public int Duration;
    public async virtual void ApplyEffect(EntityCombatController victim)
    {
        while (Duration > 0)
        {
            await Task.Delay(10);
            Duration -= 10;
        }
        OnEffectTimerEnd?.Invoke();
    }

    public static StatusEffect operator +(StatusEffect first, StatusEffect second)
    {
        //When added effects that are not the same intensity
        if (first.Intensity != second.Intensity)
        {
            StatusEffect higherIntensityEffect = first.Intensity > second.Intensity ? first : second;
            StatusEffect lowerIntensityEffect = first.Intensity < second.Intensity ? first : second;

            var intensityDifference = higherIntensityEffect.Intensity - lowerIntensityEffect.Intensity;
            var addedDuration = Math.Pow(0.5f, intensityDifference) * lowerIntensityEffect.Duration;
            first.Intensity = higherIntensityEffect.Intensity;
            first.Duration = (int)(higherIntensityEffect.Duration + addedDuration);
            return first;
        }
        first.Duration += second.Duration;
        return first;
    }
    public void AddEffect(StatusEffect other)
    {
        //When added effects that are not the same intensity
        if (Intensity != other.Intensity)
        {
            StatusEffect higherIntensityEffect = Intensity > other.Intensity ? this : other;
            StatusEffect lowerIntensityEffect = Intensity < other.Intensity ? this : other;

            var intensityDifference = higherIntensityEffect.Intensity - lowerIntensityEffect.Intensity;
            var addedDuration = Math.Pow(0.5f, intensityDifference) * lowerIntensityEffect.Duration;
            Intensity = higherIntensityEffect.Intensity;
            Duration = (int)(higherIntensityEffect.Duration + addedDuration);
            return;
        }
        Duration += other.Duration;
    }

    public abstract bool Equals(StatusEffect other);

    public bool Equals(StatusEffect x, StatusEffect y)
    {
        return x.Equals(y);
    }

    public override string ToString()
    {
        return $"Intensity: {Intensity} Duration: {Duration} ms";
    }

    public int GetHashCode(StatusEffect effect)
    {
        return effect.GetType().GetHashCode();
    }
}

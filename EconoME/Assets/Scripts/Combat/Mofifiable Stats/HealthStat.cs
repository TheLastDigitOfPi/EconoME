using System;
using UnityEngine;
public class HealthStat : ModifiableFloatStat
{
    public event Action OnFullHeal;
    public event Action OnNoHealth;
    public event Action<float> OnHeathChange;
    [field: SerializeField] public float CurrentHealth { get; private set; }
    public AntiHeal AntiHeal { get; set; }

    public void Damage(float value)
    {
        if (CurrentHealth <= 0)
            return;
        var initilaValue = value;
        var initialHealth = CurrentHealth;
        CurrentHealth -= value;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            OnNoHealth?.Invoke();
        }
        OnHeathChange?.Invoke(CurrentHealth - initialHealth);
    }

    public void Heal(float value)
    {
        if (CurrentHealth == CurrentValue)
            return;
        var initilaValue = value;
        var initialHealth = CurrentHealth;
        if (AntiHeal != null)
        {
            var percentLost = AntiHeal.Intensity > 5 ? 1f : AntiHeal.Intensity * .2f;
            value -= value * percentLost;
        }
        CurrentHealth += value;
        if (CurrentHealth >= CurrentValue)
        {
            CurrentHealth = CurrentValue;
            OnFullHeal?.Invoke();
        }
        OnHeathChange?.Invoke(CurrentHealth - initialHealth);
    }
    protected override void RecalculateStat()
    {
        var oldMaxHP = CurrentValue;
        base.RecalculateStat();
        var healthDifference = CurrentValue - oldMaxHP;
        //When increasing max health, add 50% add additional HP as health
        if (healthDifference > 0)
            CurrentHealth += healthDifference * .5f;
        //If losing max health, cap current health to new max value
        if (CurrentHealth > CurrentValue)
            CurrentHealth = CurrentValue;
    }
    public override string ToString()
    {
        return base.ToString() + $" Current health is {CurrentHealth}";
    }
}


public class EnhancedHealth : HealthStat
{
}
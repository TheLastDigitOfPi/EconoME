using System;

public class ModifiableFloatStat : ModifiableStat<float>
{
    protected override void RecalculateStat()
    {
        CurrentValue = BaseValue;
        foreach (var change in changes)
        {
            CurrentValue += change.BaseAdditionChange;
            CurrentValue += change.BasePercentChange / 100f * BaseValue;
        }
        float newBaseValue = CurrentValue;
        foreach (var change in changes)
        {
            CurrentValue += change.AddtionChange;
            CurrentValue += change.PerecentChange / 100f * CurrentValue;
        }
    }
    public override void AddChange(StatChanger change)
    {
        base.AddChange(change);
        Console.WriteLine("Stat Modified" + ToString());
    }
    public override void RemoveChange(StatChanger change)
    {
        base.RemoveChange(change);
        Console.WriteLine("Stat Modified" + ToString());
    }
    public override string ToString()
    {
        return $"Base value is: {BaseValue} and Current Value is {CurrentValue}";
    }
}

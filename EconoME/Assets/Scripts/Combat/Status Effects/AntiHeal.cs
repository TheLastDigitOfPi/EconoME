public class AntiHeal : StatusEffect
{
    public override void ApplyEffect(EntityCombatController victim)
    {
        base.ApplyEffect(victim);
        victim.Health.AntiHeal = this;
        OnEffectTimerEnd += () => victim.Health.AntiHeal = null;
    }
    public override bool Equals(StatusEffect other)
    {
        return other is AntiHeal;
    }

    public AntiHeal(int intensity, int duration)
    {
        Intensity = intensity;
        Duration = duration;
    }
}
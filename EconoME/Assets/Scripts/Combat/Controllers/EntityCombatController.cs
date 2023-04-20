using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityCombatController : MonoBehaviour, IDamageable
{
    private void Start()
    {
        Health.OnHeathChange += OnHealthChange;
    }

    private void OnHealthChange(float damage)
    {
        DamageIndicatorManager.Instance.CreateIndicator(damage, this);
    }
    #region Stats
    [field:SerializeReference] public virtual HealthStat Health { get; protected set; } = new HealthStat();
    #endregion
    HashSet<StatusEffect> _effects = new();
    public void AddEffect(StatusEffect effect)
    {
        if (_effects.TryGetValue(effect, out var foundEffect))
        {
            foundEffect.AddEffect(effect);
            Console.WriteLine("Updated Effect");
            return;
        }
        _effects.Add(effect);
        effect.OnEffectTimerEnd += RemoveEffect;
        effect.ApplyEffect(this);
        Console.WriteLine("Added Effect");
        void RemoveEffect()
        {
            _effects.Remove(effect);
            Console.WriteLine("Effect Expired");
        }
    }
    [field: SerializeField] public CombatEntityType EntityType { get; protected set; }
    
    public MoveDirection FacingDirection { get; protected set; }
    public event Action OnDeath;
    protected Action onDeath { get { return OnDeath; } set { OnDeath = value; } }
    public event Action OnDefense;
    protected Action onDefense { get { return OnDefense; } set { OnDefense = value; } }
    public virtual string GetDescription()
    {
        return "Default Entity Description :)";
    }
    public virtual bool TryReceiveAttack(Action<EntityCombatController> onAttack, out CombatDamageReport report)
    {
        report = null;
        onAttack?.Invoke(this);
        return true;
    }
}

public interface IDefenseHolder
{
    public void AddStatChange(DefenseStatChangeInstance changes);
    public void RemoveStatChange(DefenseStatChangeInstance changes);
}
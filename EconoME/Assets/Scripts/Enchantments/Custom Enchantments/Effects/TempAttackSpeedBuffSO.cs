using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Temp Attack Speed Buff", menuName = "ScriptableObjects/Enchants/Armor/Custom/Effects/Attack Speed")]
public class TempAttackSpeedBuffSO : ArmorEffectSO
{
    [SerializeField] StatChanger _attackSpeedModifier;
    [SerializeField] float _buffTime;

    public override bool TryGetEffect(EntityCombatController owner, out ArmorEffect effect)
    {
        effect = null;
        if (owner is not PlayerCombatController)
            return false;
        var player = owner as PlayerCombatController;
        effect = new TempIncreaseAttackSpeed(player, _attackSpeedModifier, _buffTime);
        return true;
    }
}

public class TempIncreaseAttackSpeed : ArmorEffect
{
    PlayerCombatController _owner;
    [SerializeField] StatChanger _attackSpeedModifier;
    [SerializeField] float _buffTime;
    [SerializeField] bool _currentlyActive = false;
    Coroutine currentBuffRoutine;
    public TempIncreaseAttackSpeed(PlayerCombatController owner, StatChanger attackSpeedModifier, float buffTime)
    {
        _attackSpeedModifier = attackSpeedModifier;
        _owner = owner;
        _buffTime = buffTime;
    }

    public override void OnUnequip()
    {
        if (_currentlyActive)
        {
            _owner.StopCoroutine(currentBuffRoutine);
            _owner.AttackSpeed.RemoveChanger(_attackSpeedModifier);
            return;
        }
    }

    public override void TriggerEffect()
    {
        if (_currentlyActive)
        {
            _owner.StopCoroutine(currentBuffRoutine);
            _owner.AttackSpeed.RemoveChanger(_attackSpeedModifier);
            currentBuffRoutine = _owner.StartCoroutine(GiveTempAttackSpeed());
            return;
        }
        currentBuffRoutine = _owner.StartCoroutine(GiveTempAttackSpeed());
        IEnumerator GiveTempAttackSpeed()
        {
            _currentlyActive = true;
            _owner.AttackSpeed.AddChanger(_attackSpeedModifier);
            yield return new WaitForSeconds(_buffTime);
            _owner.AttackSpeed.RemoveChanger(_attackSpeedModifier);
            _currentlyActive = false;
        }
    }
}

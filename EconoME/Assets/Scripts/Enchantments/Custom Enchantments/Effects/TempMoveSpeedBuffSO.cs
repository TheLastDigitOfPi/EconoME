using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Temp Movement Speed Buff", menuName = "ScriptableObjects/Enchants/Armor/Custom/Effects/Movement Speed")]
public class TempMoveSpeedBuffSO : ArmorEffectSO
{
    [SerializeField] StatChanger _moveSpeedModifier;
    [SerializeField] float _buffTime;

    public override bool TryGetEffect(EntityCombatController owner, out ArmorEffect effect)
    {
        effect = null;
        if (owner is not PlayerCombatController)
            return false;
        var player = owner as PlayerCombatController;
        effect = new TempIncreaseMoveSpeed(player, _moveSpeedModifier, _buffTime);
        return true;
    }
}


public class TempIncreaseMoveSpeed : ArmorEffect
{
    PlayerCombatController _owner;
    [SerializeField] StatChanger _moveSpeedModifier;
    [SerializeField] float _buffTime;
    [SerializeField] bool _currentlyActive = false;
    Coroutine currentBuffRoutine;
    public TempIncreaseMoveSpeed(PlayerCombatController owner, StatChanger moveSpeedModifier, float buffTime)
    {
        _moveSpeedModifier = moveSpeedModifier;
        _owner = owner;
        _buffTime = buffTime;
    }

    public override void OnUnequip()
    {
        var controller = PlayerMovementController.Instance;
        if (_currentlyActive)
        {
            controller.StopCoroutine(currentBuffRoutine);
            controller.PlayerSpeed.RemoveChanger(_moveSpeedModifier);
            return;
        }
    }

    public override void TriggerEffect()
    {
        var controller = PlayerMovementController.Instance;
        if (_currentlyActive)
        {
            controller.StopCoroutine(currentBuffRoutine);
            controller.PlayerSpeed.RemoveChanger(_moveSpeedModifier);
            currentBuffRoutine = _owner.StartCoroutine(GiveTempAttackSpeed());
            return;
        }
        currentBuffRoutine = _owner.StartCoroutine(GiveTempAttackSpeed());
        IEnumerator GiveTempAttackSpeed()
        {
            _currentlyActive = true;
            controller.PlayerSpeed.AddChanger(_moveSpeedModifier);
            yield return new WaitForSeconds(_buffTime);
            controller.PlayerSpeed.RemoveChanger(_moveSpeedModifier);
            _currentlyActive = false;
        }
    }
}
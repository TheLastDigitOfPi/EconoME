using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "New Basic Follow And Attack AI", menuName = "ScriptableObjects/AI/States/Mob Basic Follow And Attack")]

public class BasicMobFollowAndAttackAISO : AIStateSO
{
    [SerializeReference] BasicMobFollowAndAttackAI data = new();
    public override bool GetAIState(AIController controller, out AIState state)
    {
        state = new BasicMobFollowAndAttackAI(controller, data);
        return state.PassedValidation;
    }
}



[System.Serializable]
public class BasicMobFollowAndAttackAI : AIState
{
    //Settings
    [SerializeField] Vector3Variable _playerPosition;
    Mob _mobData;
    bool inAggroRangeOfPlayer = false;
    bool inAttackRangeOfPlayer = false;
    bool followingPlayer = false;
    bool AttackAIActive = false;
    public BasicMobFollowAndAttackAI() { }
    IMobWithAttack _mobAttack;

    public BasicMobFollowAndAttackAI(AIController controller, BasicMobFollowAndAttackAI other) : base(controller, other.AICondition)
    {
        
        _playerPosition = other._playerPosition;
        //Set Settings
        if (!Controller.TryGetComponent(out _mobData))
        {
            FailedStateRequirements(this, "No Mob Class Found");
            return;
        }
        if (_mobData is not IMobWithAttack)
        {
            FailedStateRequirements(this, "Mob Class does not contain attack");
            return;
        }
        _mobAttack = _mobData as IMobWithAttack;

        _mobAttack.OnTargetEnterAggroRange += TargetInAggroRange;
        _mobAttack.OnTargetLeaveAggroRange += TargetLeaveAggroRange;
        _mobAttack.OnTargetEnterAttackRange += TargetInAttackRange;
        _mobAttack.OnTargetLeaveAttackRange += TargetLeaveAttackRange;
    }

    public override void OnEnter()
    {
        AttackAIActive = true;
    }

    private void TargetLeaveAttackRange()
    {
        inAttackRangeOfPlayer = false;
        if (!AttackAIActive)
            return;

        if(inAggroRangeOfPlayer)
            TargetInAggroRange();
    }

    private async void TargetInAttackRange()
    {
        inAttackRangeOfPlayer = true;
        if (!AttackAIActive)
            return;
        while (inAttackRangeOfPlayer)
        {
            if (_mobAttack.TryGetTarget(out var target))
                _mobAttack.TryAttack(target);
            await Task.Delay(20);
        }
    }

    private void TargetLeaveAggroRange()
    {
        inAggroRangeOfPlayer = false;
        if (!AttackAIActive)
            return;
        _mobData.MoveTowardsPosition(_mobData.transform.position);
    }

    private void TargetInAggroRange()
    {
        inAggroRangeOfPlayer = true;
        if (!AttackAIActive)
            return;
        if(!followingPlayer)
            FollowPlayer();
        async void FollowPlayer()
        {
            followingPlayer = true;
            while (inAggroRangeOfPlayer && !inAttackRangeOfPlayer)
            {
                _mobData.MoveTowardsPosition(_mobAttack.Target.transform.position - ((_mobAttack.Target.transform.position - _mobData.transform.position).normalized) * 0.6f);
                await Task.Delay(200);
            }
            followingPlayer = false;
        }
    }

    public override void OnExit()
    {
        AttackAIActive = false;
    }

    public override void Tick()
    {
        if(inAggroRangeOfPlayer)
            TargetInAggroRange();
    }


}
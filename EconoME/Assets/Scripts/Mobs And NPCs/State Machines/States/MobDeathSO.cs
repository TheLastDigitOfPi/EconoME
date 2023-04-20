using UnityEngine;

[CreateAssetMenu(fileName = "New Mob Death", menuName = "ScriptableObjects/AI/States/Mob Death")]
public class MobDeathSO : AIStateSO
{
    [SerializeReference] MobDeath data = new();
    public override bool GetAIState(AIController controller, out AIState state)
    {
        state = new MobDeath(controller, data);
        return state.PassedValidation;
    }
}

[System.Serializable]
public class MobDeath : AIState
{
    //Customizations
    [SerializeField] string _animationName = "Death";
    [SerializeField] float _timeTillDestroy = 4f;
    
    private Animator _animator;
    Mob _mobData;

    public MobDeath(){ }
    public MobDeath(AIController controller, MobDeath other) : base(controller, other.AICondition)
    {
        //Implement Settings
        _animationName = other._animationName;
        _timeTillDestroy = other._timeTillDestroy;

        //Check that all requirements are met
        if (!Controller.TryGetComponent(out _animator))
        {
            FailedStateRequirements(this, "No Animator Found");
        }

        if (!Controller.TryGetComponent<Mob>(out _mobData))
        {
            FailedStateRequirements(this, "No Mob Data Found");
        }
    }

    public override void OnEnter()
    {
        //Play death animation
        _animator.CrossFade(_animationName, 0);
    }

    public override void OnExit()
    {
        Controller.isRunning = true;
    }

    public override void Tick()
    {
        //Wait for some time before declaring mob fully dead
        _timeTillDestroy -= Time.deltaTime;
        if (_timeTillDestroy <= 0)
        {
            _mobData.OnMobDeath();
            Controller.isRunning = false;
        }
    }
}

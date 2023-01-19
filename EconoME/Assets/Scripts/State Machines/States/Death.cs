using UnityEngine;

[CreateAssetMenu(fileName = "New Mob Death", menuName = "ScriptableObjects/AI/States/Mob Death")]
public class Death : AIState
{
    private Animator _animator;
    [SerializeField] string _animationName = "Death";
    [SerializeField] float _timeTillDestroy = 4f;

    protected override void OnStart()
    {
        //Check that all requirements are met
        if (!Controller.TryGetComponent(out _animator))
        {
            Controller.FailedStateRequirements(this, "No Animator Found");
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
            MobData.OnDeath();
            Controller.isRunning = false;
        }
    }
}

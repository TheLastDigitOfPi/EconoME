using UnityEngine;

[CreateAssetMenu(fileName = "New Mob Wander", menuName = "ScriptableObjects/AI/States/Mob Wander")]
public class WanderAISO : AIStateSO
{
    [SerializeReference] WanderAI data = new();
    public override bool GetAIState(AIController controller, out AIState state)
    {
        state = new WanderAI(controller, data);
        return state.PassedValidation;
    }
}
[System.Serializable]
public class WanderAI : AIState
{
    //Settings
    [SerializeField] string _walkAnimationName = "Walk";
    [SerializeField] string _idleAnimationName = "Idle";

    private Animator _animator;
    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidBody;
    private Vector2 movePos;
    Vector2 _lastPosition = Vector2.zero;
    private float TimeStuck;
    private float WaitTime = 0f;
    float TimeToWait = 3f;
    bool Waiting = false;

    public WanderAI() { }
   
    public WanderAI(AIController controller, WanderAI other) : base(controller, other.AICondition)
    {
        //Implement Settings
        _walkAnimationName = other._walkAnimationName;
        _idleAnimationName = other._idleAnimationName; 
        //Check that all requirements are met
        if (!Controller.TryGetComponent(out _animator))
        {
            FailedStateRequirements(this, "No Animator Found");
            return;
        }

        if (!Controller.TryGetComponent(out _renderer))
        {
            FailedStateRequirements(this, "No Sprite Renderer Found");
            return;
        }

        if (!Controller.TryGetComponent(out _rigidBody))
        {
            FailedStateRequirements(this, "No RigidBody2D Found");
            return;
        }
    }

    public override void OnEnter()
    {
        _animator.CrossFade(_idleAnimationName, 0);

        Waiting = true;
        TimeStuck = 0f;
    }

    public override void OnExit()
    {
        Waiting = true;
        WaitTime = 0;
    }

    public override void Tick()
    {
        if (!Waiting)
        {
            var moveTo = Vector2.MoveTowards(_rigidBody.position, movePos, Time.deltaTime * 5);
            _rigidBody.MovePosition(moveTo);
            if (_rigidBody.position == movePos)
            {
                Waiting = true;
                _animator.CrossFade(_idleAnimationName, 0);
                return;
            }
            if (Vector2.Distance(Controller.transform.position, _lastPosition) <= 0.02f)
                TimeStuck += Time.deltaTime;
            _lastPosition = Controller.transform.position;

            if (TimeStuck > 3f)
            {
                Waiting = true;
                _animator.CrossFade(_idleAnimationName, 0);
            }
            return;
        }
        WaitTime += Time.deltaTime;
        if (WaitTime > TimeToWait)
        {
            Waiting = false;
            WaitTime = 0f;
            TimeToWait = UnityEngine.Random.Range(2f, 5f);
            MoveToNewPosition();
        }
    }
    void MoveToNewPosition()
    {
        _animator.CrossFade(_walkAnimationName, 0);
        TimeStuck = 0f;
        movePos = _rigidBody.position + VectorExtensions.GetRandomDir() * UnityEngine.Random.Range(1f, 2f);

        _renderer.flipX = movePos.x > Controller.transform.position.x ? false : true;

    }
}

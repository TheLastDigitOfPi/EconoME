using UnityEngine;

[CreateAssetMenu(fileName = "New Basic Attack AI", menuName = "ScriptableObjects/AI/States/Mob Basic Attack AI")]

public class BasicAttackAISO : AIStateSO
{
    [SerializeReference] BasicAttackAI data = new();

    public override bool GetAIState(AIController controller, out AIState state)
    {
        state = new BasicAttackAI(controller, data);
        return state.PassedValidation;
    }
}
[System.Serializable]
public class BasicAttackAI : AIState
{
    [SerializeField] string _walkAnimationName = "Walk";
    [SerializeField] Vector3Variable _playerPosition;
    [SerializeField] float _circleRadius = 1;
    [SerializeField] float _rotationSpeed = 1;

    private Animator _animator;
    private float angle;
    private SpriteRenderer _renderer;
    Mob _mobData;

    private Rigidbody2D _rigidBody;
    private Vector2 movePos;
    Vector2 _lastPosition = Vector2.zero;
    private float TimeStuck;

    public BasicAttackAI() { }
    public BasicAttackAI(AIController controller, BasicAttackAI other) : base(controller, other.AICondition)
    {
        //Implement Settings
        _walkAnimationName = other._walkAnimationName;
        _circleRadius = other._circleRadius;
        _rotationSpeed = other._rotationSpeed;
        _playerPosition = other._playerPosition;

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

        if (!Controller.TryGetComponent<Mob>(out _mobData))
        {
            FailedStateRequirements(this, "No Mob Data Found");
            return;
        }
    }

    public override void OnEnter()
    {
        angle = Vector3.Angle(_playerPosition.Value, _rigidBody.position);
        _animator.CrossFade(_walkAnimationName, 0);
    }

    public override void OnExit()
    {
    }

    public override void Tick()
    {

        if (Vector2.Distance(Controller.transform.position, _lastPosition) <= 0.02f)
            TimeStuck += Time.deltaTime;
        _lastPosition = Controller.transform.position;
        Vector3 positionOffset = new();
        if (Vector3.Distance(_rigidBody.position, _playerPosition.Value) <= _circleRadius)
        {
            positionOffset.Set(Mathf.Cos(angle) * _circleRadius, Mathf.Sin(angle) * _circleRadius, 0);
            _rigidBody.MovePosition(Vector2.MoveTowards(_rigidBody.position, _playerPosition.Value + positionOffset, Time.deltaTime * _mobData.Speed));
            angle += Time.deltaTime * _mobData.Speed;
            _renderer.flipX = _playerPosition.Value.x > Controller.transform.position.x ? false : true;
            return;
        }

        var moveTo = Vector2.MoveTowards(_rigidBody.position, _playerPosition.Value + positionOffset, Time.deltaTime * _mobData.Speed);
        _rigidBody.MovePosition(moveTo);
        _renderer.flipX = _playerPosition.Value.x > Controller.transform.position.x ? (_mobData.Speed < 0) : (_mobData.Speed >= 0);

        return;
    }
}


using UnityEngine;

[CreateAssetMenu(fileName = "New Basic Attack AI", menuName = "ScriptableObjects/AI/States/Mob Basic Attack AI")]
public class BasicAttackAI : AIState
{
    private Animator _animator;
    [SerializeField] string _walkAnimationName = "Walk";
    [SerializeField] Vector3Variable playerPosition;
    [SerializeField] float _circleRadius = 1;
    [SerializeField] float _rotationSpeed = 1;

    private float angle;
    private SpriteRenderer _renderer;
    protected override void OnStart()
    {
        //Check that all requirements are met
        if (!Controller.TryGetComponent(out _animator))
        {
            Controller.FailedStateRequirements(this, "No Animator Found");
            return;
        }

        if (!Controller.TryGetComponent(out _renderer))
        {
            Controller.FailedStateRequirements(this, "No Sprite Renderer Found");
            return;
        }

        if (!Controller.TryGetComponent(out _rigidBody))
        {
            Controller.FailedStateRequirements(this, "No RigidBody2D Found");
            return;
        }

    }

    private Rigidbody2D _rigidBody;
    private Vector2 movePos;
    Vector2 _lastPosition = Vector2.zero;
    private float TimeStuck;

    public override void OnEnter()
    {
        angle = Vector3.Angle(playerPosition.Value, _rigidBody.position);
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
        if (Vector3.Distance(_rigidBody.position, playerPosition.Value) <= _circleRadius)
        {
            positionOffset.Set(Mathf.Cos(angle) * _circleRadius, Mathf.Sin(angle) * _circleRadius, 0);
            _rigidBody.MovePosition(Vector2.MoveTowards(_rigidBody.position, playerPosition.Value + positionOffset, Time.deltaTime * MobData.Speed));
            angle += Time.deltaTime * MobData.Speed;
            _renderer.flipX = playerPosition.Value.x > Controller.transform.position.x ? false : true;
            return;
        }

        var moveTo = Vector2.MoveTowards(_rigidBody.position, playerPosition.Value + positionOffset, Time.deltaTime * MobData.Speed);
        _rigidBody.MovePosition(moveTo);
        _renderer.flipX = playerPosition.Value.x > Controller.transform.position.x ? (MobData.Speed < 0) : (MobData.Speed >= 0);

        return;
    }
}

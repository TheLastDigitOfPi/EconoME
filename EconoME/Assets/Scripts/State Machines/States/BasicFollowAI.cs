using UnityEngine;

[CreateAssetMenu(fileName = "New Basic Follow AI", menuName = "ScriptableObjects/AI/States/Mob Basic Follow AI")]
public class BasicFollowAI : AIState
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
    Vector2 _lastPosition = Vector2.zero;
    private float TimeStuck;
    Collider2D[] selfColliders;

    public override void OnEnter()
    {
        angle = Vector3.Angle(playerPosition.Value, _rigidBody.position);
        _animator.CrossFade(_walkAnimationName, 0);
        selfColliders = _rigidBody.GetComponents<Collider2D>();
    }

    public override void OnExit()
    {
    }

    public override void Tick()
    {

        //Check if obstacle is in front of mob

        bool isOwnedCollider(Collider2D collider)
        {
            foreach (var ownedCollider in selfColliders)
            {
                if (collider == ownedCollider) { return true; }
            }
            return false;
        }


        bool RaycastHitObstacle(RaycastHit2D[] hits)
        {
            foreach (var hit in hits)
            {
                if (hit.collider != null && !hit.collider.CompareTag("Player") && !isOwnedCollider(hit.collider))
                {
                    Debug.Log("Obstacle in way named " + hit.collider.gameObject.name);
                    return true;
                }
            }
            return false;
        }

        Vector2 CanMoveInDireciton(Vector2 position)
        {
            var direction = (position - _rigidBody.position).normalized;
            var raycast = Physics2D.RaycastAll(_rigidBody.position, direction, 2);
            Debug.DrawRay(_rigidBody.position, direction, Color.white);
            if (RaycastHitObstacle(raycast))
            {
                //Obstacle in the way
                float initalAngle = Vector2.Angle(_rigidBody.position, direction);
                for (float i = initalAngle; i < initalAngle + 90; i++)
                {
                    var checkClockwise = Physics2D.RaycastAll(_rigidBody.position, i.DegreeToVector2(), 2);
                    if (!RaycastHitObstacle(checkClockwise))
                    {
                        //Valid direction, move this way
                        return position.Rotate(i); ;
                    }
                    var checkCounterClockwise = Physics2D.RaycastAll(_rigidBody.position, -i.DegreeToVector2(), 2);
                    if (!RaycastHitObstacle(checkCounterClockwise))
                    {
                        return position.Rotate(-i);
                    }
                }

                return Vector2.positiveInfinity;
            }

            return position;
        }


        var movePosition = playerPosition.Value.ToVector2();
        //If stuck attempt to find new angle to move at
        if (Vector2.Distance(Controller.transform.position, _lastPosition) <= 0.02f)
            TimeStuck += Time.deltaTime;

        _lastPosition = Controller.transform.position;
        Vector3 positionOffset = new();
        if (Vector3.Distance(_rigidBody.position, playerPosition.Value) <= _circleRadius)
        {
            positionOffset.Set(Mathf.Cos(angle) * _circleRadius, Mathf.Sin(angle) * _circleRadius, 0);
            movePosition = CanMoveInDireciton(playerPosition.Value + positionOffset);

            //If move position is blocked, stop moving
            if (movePosition == Vector2.positiveInfinity)
            {
                Debug.Log("No LOS to target, not moving");
                return;
            }

            _rigidBody.MovePosition(Vector2.MoveTowards(_rigidBody.position, movePosition, Time.deltaTime * MobData.Speed));
            angle += Time.deltaTime * MobData.Speed;
            _renderer.flipX = playerPosition.Value.x <= Controller.transform.position.x;
            return;

        }

        movePosition = CanMoveInDireciton(movePosition);

        if (movePosition.Equals(Vector2.positiveInfinity))
        {
            Debug.Log("No LOS to target, not moving");
            return;
        }
        var moveTo = Vector2.MoveTowards(_rigidBody.position, movePosition, Time.deltaTime * MobData.Speed);
        _rigidBody.MovePosition(moveTo);
        _renderer.flipX = playerPosition.Value.x > Controller.transform.position.x ? (MobData.Speed < 0) : (MobData.Speed >= 0);

        return;
    }
}

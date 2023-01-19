using System;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Basic Follow AI NavMesh", menuName = "ScriptableObjects/AI/States/Mob Basic Follow AI NavMesh")]
public class BasicFollowAINavMesh : AIState
{
    private Animator _animator;
    [SerializeField] string _walkAnimationName = "Walk";
    [SerializeField] string _idleAnimationName = "Idle";
    [SerializeField] Vector3Variable playerPosition;
    [SerializeField] float _circleRadius = 1;
    [SerializeField] float _circleRadiusMax = 2;

    private Vector2 _currentPathPosition;
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
        /*
        if (!Controller.TryGetComponent(out _rigidBody))
        {
            Controller.FailedStateRequirements(this, "No RigidBody2D Found");
            return;
        }
        */
    }

    //private Rigidbody2D _rigidBody;

    public override void OnEnter()
    {
        Controller.NavMeshAgent.SetDestination(playerPosition.Value);
        _animator.CrossFade(_walkAnimationName, 0);
        angle = Vector3.Angle(playerPosition.Value, Controller.transform.position);
    }

    public override void OnExit()
    {

    }

    public override void Tick()
    {

        //Circle around player if near them
        if (Vector3.Distance(Controller.transform.position, playerPosition.Value) <= _circleRadiusMax)
        {
            CircleAroundPlayer();
            return;
        }

        //Otherwise move towards the player
        if (Controller.NavMeshAgent.SetDestination(playerPosition.Value))
        {
            MoveTowardsPlayer();
        }


        void CircleAroundPlayer()
        {
            //Get the position of the circle we want to rotate around
            Vector3 positionOffset = new();
            positionOffset.Set(Mathf.Cos(angle) * _circleRadius, Mathf.Sin(angle) * _circleRadius, 0);
            var movePosition = playerPosition.Value + positionOffset;
            //Update angle
            angle += Time.deltaTime * MobData.Speed;
            //Move to destination
            Controller.NavMeshAgent.SetDestination(movePosition);
            _currentPathPosition = movePosition;
            _renderer.flipX = _currentPathPosition.x <= Controller.transform.position.x;
        }

        void MoveTowardsPlayer()
        {
            _currentPathPosition = playerPosition.Value;
            _renderer.flipX = _currentPathPosition.x <= Controller.transform.position.x;
        }
    }


}
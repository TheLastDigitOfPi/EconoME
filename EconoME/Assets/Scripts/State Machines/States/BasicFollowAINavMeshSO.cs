using System;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Basic Follow AI NavMesh", menuName = "ScriptableObjects/AI/States/Mob Basic Follow AI NavMesh")]

public class BasicFollowAINavMeshSO : AIStateSO
{
    [SerializeReference] BasicFollowAINavMesh data = new();
    public override bool GetAIState(AIController controller, out AIState state)
    {
        state = new BasicFollowAINavMesh(controller, data);
        return state.PassedValidation;
    }
}

[System.Serializable]
public class BasicFollowAINavMesh : AIState
{
    //Settings
    [SerializeField] string _walkAnimationName = "Walk";
    [SerializeField] string _idleAnimationName = "Idle";
    [SerializeField] Vector3Variable _playerPosition;
    [SerializeField] float _circleRadius = 1;
    [SerializeField] float _circleRadiusMax = 2;

    private Animator _animator;
    private Vector2 _currentPathPosition;
    private float angle;
    private SpriteRenderer _renderer;
    Mob _mobData;

    public BasicFollowAINavMesh() { }
    

    public BasicFollowAINavMesh(AIController controller, BasicFollowAINavMesh other) : base(controller, other.AICondition)
    {
        //Set Settings
        _walkAnimationName = other._walkAnimationName;
        _idleAnimationName = other._idleAnimationName;
        _circleRadius = other._circleRadius;
        _playerPosition = other._playerPosition;
        _circleRadiusMax = other._circleRadiusMax;

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

        if (!Controller.TryGetComponent(out _mobData))
        {
            FailedStateRequirements(this, "No Mob Class Found");
            return;
        }
    }


    public override void OnEnter()
    {
        Controller.NavMeshAgent.SetDestination(_playerPosition.Value);
        _animator.CrossFade(_walkAnimationName, 0);
        angle = Vector3.Angle(_playerPosition.Value, Controller.transform.position);
    }

    public override void OnExit()
    {

    }

    public override void Tick()
    {

        //Circle around player if near them
        if (Vector3.Distance(Controller.transform.position, _playerPosition.Value) <= _circleRadiusMax)
        {
            CircleAroundPlayer();
            return;
        }

        //Otherwise move towards the player
        if (Controller.NavMeshAgent.SetDestination(_playerPosition.Value))
        {
            MoveTowardsPlayer();
        }


        void CircleAroundPlayer()
        {
            //Get the position of the circle we want to rotate around
            Vector3 positionOffset = new();
            positionOffset.Set(Mathf.Cos(angle) * _circleRadius, Mathf.Sin(angle) * _circleRadius, 0);
            var movePosition = _playerPosition.Value + positionOffset;
            //Update angle
            angle += Time.deltaTime * _mobData.Speed;
            //Move to destination
            Controller.NavMeshAgent.SetDestination(movePosition);
            _currentPathPosition = movePosition;
            _renderer.flipX = _currentPathPosition.x <= Controller.transform.position.x;
        }

        void MoveTowardsPlayer()
        {
            _currentPathPosition = _playerPosition.Value;
            _renderer.flipX = _currentPathPosition.x <= Controller.transform.position.x;
        }
    }


}
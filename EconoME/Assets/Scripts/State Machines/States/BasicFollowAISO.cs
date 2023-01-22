using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Basic Follow AI Raycasting", menuName = "ScriptableObjects/AI/States/Mob Follow Raycast AI")]

public class BasicFollowAISO : AIStateSO
{
    [SerializeReference] BasicFollowAI data = new();

    public override bool GetAIState(AIController controller, out AIState state)
    {
        state = new BasicFollowAI(controller, data);
        return state.PassedValidation;
    }
}

[System.Serializable]
public class BasicFollowAI : AIState
{
    [SerializeField] string _walkAnimationName = "Walk";
    [SerializeField] string _idleAnimationName = "Idle";
    [SerializeField] Vector3Variable _playerPosition;
    [SerializeField] float _circleRadius = 1;
    [SerializeField] float _rotationSpeed = 1;

    private Animator _animator;
    Mob _mobData;
    private Rigidbody2D _rigidBody;
    public BasicFollowAI() { }
    public BasicFollowAI(AIController controller, BasicFollowAI other) : base(controller, other.AICondition)
    {
        //Implement Settings
        _walkAnimationName = other._walkAnimationName;
        _circleRadius = other._circleRadius;
        _rotationSpeed = other._rotationSpeed;
        _playerPosition = other._playerPosition;
        _pathValidationInterval = other._pathValidationInterval;
        _obstacleDegreeCheck = other._obstacleDegreeCheck;
        stoppingDistance = other.stoppingDistance;
        _pathWidth = other._pathWidth;
        controller.onDrawGizmos += () => Gizmos.DrawCube(_currentMovePos, new Vector3(0.1f, 0.1f, 1f));

        //Check that all requirements are met
        if (!Controller.TryGetComponent(out _animator))
        {
            FailedStateRequirements(this, "No Animator Found");
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
        _animator.CrossFade(_walkAnimationName, 0);
    }

    public override void OnExit()
    {
    }
    float _currentPathValidationTime;

    [SerializeField] float _pathValidationInterval = 0.5f;
    [SerializeField] float _obstacleDegreeCheck = 90f;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] float _pathWidth = 0.1f;

    [SerializeField] float _checkForNewPathTime = 3f;
    float _currentCheckForNewPathTime = 0;
    [SerializeField] List<Vector2> _pathPoints = new();

    bool HasPath { get { return _pathPoints.Count > 0; } }
    bool NearPlayer { get { return Vector2.Distance(_playerPosition.Value, _rigidBody.position) < stoppingDistance; } }
    Vector2 _currentMovePos { get { return _pathPoints.Count > 0 ? _pathPoints[0] : _rigidBody.position; } }

    public override void Tick()
    {
        if (NearPlayer)
        {
            _animator.CrossFade(_idleAnimationName, 0);
            _pathPoints.Clear();
            return;
        }
        if (HasPath)
        {
            MoveTowardsPath();
            _animator.CrossFade(_walkAnimationName, 0);
            _currentPathValidationTime -= Time.deltaTime;
            if (_currentPathValidationTime <= 0)
                UpdatePath();
            return;
        }
        _currentCheckForNewPathTime -= Time.deltaTime;
        if(_currentCheckForNewPathTime <=0)
            GetNewPath();

        void UpdatePath()
        {
            _currentPathValidationTime = _pathValidationInterval;

            //Check if path points are still valid

            if (!checkPathPoint(_rigidBody.position, _currentMovePos))
                return;

            for (int i = 0; i < _pathPoints.Count - 1; i++)
            {
                if (!checkPathPoint(_pathPoints[i], _pathPoints[i + 1]))
                    return;
            }

            bool checkPathPoint(Vector2 pointA, Vector2 pointB)
            {
                Vector2 currentAngle = (pointB - pointA).normalized;
                float currentDistance = Vector2.Distance(pointB, pointA);

                //Raycast from this point to the next, if it is still valid then continue
                var currentHits = Physics2D.RaycastAll(pointA, currentAngle, currentDistance);
                Debug.DrawLine(pointA, pointB, Color.green);
                foreach (var hit in currentHits)
                {
                    if (hit.collider.CompareTag("Obstacle"))
                    {
                        GetPathFromPoint(_pathPoints.IndexOf(pointB));
                        return false;
                    }
                    if (hit.collider.CompareTag("Player"))
                    {
                        _pathPoints.RemoveRange(_pathPoints.IndexOf(pointB), _pathPoints.Count - 1);
                        _pathPoints.Add(_playerPosition.Value);
                        return false;
                    }
                }
                return true;
            }
        }

        void MoveTowardsPath()
        {
            if (Vector2.Distance(_rigidBody.position, _currentMovePos) < 0.05f)
                _pathPoints.RemoveAt(0);

            if (HasPath)
                _rigidBody.MovePosition(Vector2.MoveTowards(_rigidBody.position, _currentMovePos, Time.deltaTime * _mobData.Speed));
        }

        void GetPathFromPoint(int point)
        {
            _pathPoints.RemoveRange(point, _pathPoints.Count);
            int tooManyLoops = 10;
            while (!GetNextPathPoint(_playerPosition.Value) && tooManyLoops > 0)
                tooManyLoops--;
        }

        void GetNewPath()
        {
            _currentPathValidationTime = _pathValidationInterval;
            _currentCheckForNewPathTime = _checkForNewPathTime;
            _pathPoints.Clear();
            _pathPoints.Add(_rigidBody.position);
            int tooManyLoops = 10;
            while (!GetNextPathPoint(_playerPosition.Value) && tooManyLoops > 0)
                tooManyLoops--;

        }
        //Return true when path finishes or fails. False if found a point and it is not the final destination (player)
        bool GetNextPathPoint(Vector2 destination)
        {
            Vector2 angle = (destination - _pathPoints[_pathPoints.Count - 1]).normalized;
            float distance = Vector2.Distance(destination, _pathPoints[_pathPoints.Count - 1]);

            //Raycast to player, if path is available, move towards player, set the player position to our current path
            var hits = Physics2D.RaycastAll(_pathPoints[_pathPoints.Count - 1], angle, distance);
            Debug.DrawLine(_pathPoints[_pathPoints.Count - 1], _pathPoints[_pathPoints.Count - 1] + (angle * distance), Color.cyan);
            RaycastHit2D obstacleHit = new();
            if (CheckingPath(hits))
                return true;

            bool CheckingPath(RaycastHit2D[] hits)
            {
                foreach (var hit in hits)
                {
                    if (hit.collider.CompareTag("Obstacle"))
                    {
                        obstacleHit = hit;
                        return false;
                    }
                    if (hit.collider.CompareTag("Player"))
                    {
                        _pathPoints.Add(destination);
                        return true;
                    }
                }
                return false;
            }

            //If we hit nothing and somehow missed the player (idk how) set path to invalid
            if (hits.Length == 0)
            {
                Debug.Log("Somehow unable to raycast from mob to player");
                return true;
            }

            //Otherwise check for shortest angle to new path
            distance = Vector2.Distance(_pathPoints[_pathPoints.Count - 1], obstacleHit.point);
            for (float i = 0; i < _obstacleDegreeCheck / 2; i++)
            {
                var totalDistance = distance + Mathf.Deg2Rad * i * distance;
                hits = Physics2D.RaycastAll(_pathPoints[_pathPoints.Count - 1], angle.Rotate(i), totalDistance);
                Debug.DrawLine(_pathPoints[_pathPoints.Count - 1], _pathPoints[_pathPoints.Count - 1] + (angle.Rotate(i) * totalDistance), Color.blue);
                if (foundPath(hits, angle.Rotate(i), totalDistance))
                    return true;

                hits = Physics2D.RaycastAll(_pathPoints[_pathPoints.Count - 1], angle.Rotate(-i), totalDistance);
                Debug.DrawLine(_pathPoints[_pathPoints.Count - 1], _pathPoints[_pathPoints.Count - 1] + (angle.Rotate(-i) * totalDistance), Color.red);
                if (foundPath(hits, angle.Rotate(-i), totalDistance))
                    return true;
            }
            return false;

            //Check the raycast to see if it can be a valid point to path to
            bool foundPath(RaycastHit2D[] hits, Vector2 direction, float newDistance)
            {
                //Check if path directly hits player or hits an obstacle, if hits an obstacle then it's an invalid path
                foreach (var hit in hits)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        _pathPoints.Add(destination);
                        return true;
                    }

                    if (hit.collider.CompareTag("Obstacle"))
                    {
                        return false;
                    }
                }
                //Otherwise the point is valid
                //See if that point can see the player

                Vector2 pointPos = _pathPoints[_pathPoints.Count - 1] + (direction * newDistance);
                Vector2 newAngle = (destination - pointPos).normalized;
                float newPointToPlayerDistance = Vector2.Distance(pointPos, destination);
                var newHits = Physics2D.RaycastAll(pointPos, newAngle, newPointToPlayerDistance);
                Debug.DrawLine(pointPos, destination, Color.black);
                foreach (var hit in newHits)
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        _pathPoints.Add(pointPos);
                        return true;
                    }

                    if (hit.collider.CompareTag("Obstacle"))
                    {
                        return false;
                    }
                }
                return false;
            }
        }

        #region Thoughts
/*
        Vector2 CurrentMovPos
        PlayerPos

        if I have a path
            MoveTowardsPath()
            count up time before checking if path is still valid
            if the path is not valid (obstacle now in way)
                Get A New Path
                

        if I don't have a path

        Get A Path
        {
            Raycast to player, if path is available, move towards player, set the player position to our current path
            return
        }

        GetAPath()
        {
            Raycast to player, if path is available set it, set the player position to our current path
            
            otherwise
            
            raycast again + and - variable for amount to move angle by (smaller number means heavier hit on performance)
                if raycast hits player, MoveTowardsPath
                if raycast hits nothing, check if that position can move towards player
                    if can, set that point to be the path
            if no angle is found, no valid path, be sad
            
            reset time checking
        }

        MoveTowardsPath()
        GetAPath()
         
         */
        #endregion
        
    }
}
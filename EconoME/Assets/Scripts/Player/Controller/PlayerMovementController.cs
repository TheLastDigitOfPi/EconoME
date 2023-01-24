using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    //Static
    public static PlayerMovementController Instance;

    //Events

    public event Action OnEndUseTool;
    public event Action OnStartUseTool;

    public event Action OnPlayerStartMove;
    public event Action OnPlayerStopMove;

    public event Action OnPlayerStartSprint;
    public event Action OnPlayerStopSprint;
    public event Action OnPlayerChangeDirection;

    //Public Fields
    [field: SerializeField] public bool PlayerSprinting { get; private set; }
    [field: SerializeField] public bool UsingTool { get; private set; } = false;
    [field: SerializeField] public Vector2 MoveVec { get; private set; } = Vector2.zero;
    public bool IsWalking { get { return (!PlayerSprinting && (MoveVec.y != 0 || MoveVec.x != 0)); } }
    public MoveDirection CurrentFacingDirection { get; private set; } = MoveDirection.Right;
    public MoveDirection CurrentRaycastDirection { get; private set; } = MoveDirection.Right;

    public bool PlayerWantsToBeMovingButCant { get; private set; }
    public bool PlayerMoving { get; private set; }

    //Local Fields
    [SerializeField] float _baseSpeed;
    [SerializeField] float _runSpeedModifier = 1.5f;
    [SerializeField] public Vector3Variable PlayerPosition;
    [SerializeField] BoolListVariable UIOpen;
    Rigidbody2D _rigidbody2D;
    private MoveDirection _lastDirection = MoveDirection.Right;
    bool _playerMoveKeyPressed;
    bool _playerSprintKeyPressed;

    //Controls
    private PlayerInput playerInput;
    private InputAction _moveAction;
    private InputAction _sprintAction;

    //Helpers
    private float CurrentSpeed { get { return PlayerSprinting ? _baseSpeed * _runSpeedModifier : _baseSpeed; } }
    public MoveDirection CurrentDirection
    {
        get
        {
            if (MoveVec.y > 0) { _lastDirection = MoveDirection.Up; }
            if (MoveVec.y < 0) { _lastDirection = MoveDirection.Down; }
            if (MoveVec.x < 0) { _lastDirection = MoveDirection.Left; }
            if (MoveVec.x > 0) { _lastDirection = MoveDirection.Right; }
            return _lastDirection;
        }

    }
    public bool CanMove
    {
        get
        {
            return !UIOpen.TrueValueExists() && !UsingTool && !GlobalSceneManager.Instance.SceneTransitioning;
        }
    }


    public void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 Player Controller found active!");
            Destroy(this);
            return;
        }
        Instance = this;

        _rigidbody2D = GetComponent<Rigidbody2D>();

        //Player Input
        playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"];
        _sprintAction = playerInput.actions["Sprint"];

    }

    private void Start()
    {
        GlobalSceneManager.OnLocationChange += LocationChange;
    }
    private void LocationChange(WorldLocationData location)
    {
        if (!location.TryGetTransitionWaypoint(out WorldWayPointData waypoint))
            return;

        var pos = waypoint.WayPointWorldPosition;
        Debug.Log("x:" + pos.x + " y:" + pos.y + " z:" + pos.z);

        _rigidbody2D.position = pos;
        PlayerPosition.Value = transform.position;
    }

    private void OnEnable()
    {
        _sprintAction.performed += SprintStartKey;
        _sprintAction.canceled += SprintEndKey;
        _moveAction.started += StartedMovingKey;
        _moveAction.canceled += StoppedMovingKey;
    }

    private void StoppedMovingKey(InputAction.CallbackContext obj)
    {
        _playerMoveKeyPressed = false;
    }

    private void StartedMovingKey(InputAction.CallbackContext obj)
    {
        _playerMoveKeyPressed = true;
    }

    private void OnDisable()
    {
        _sprintAction.performed -= SprintStartKey;
        _sprintAction.canceled -= SprintEndKey;
        _moveAction.started -= StartedMovingKey;
        _moveAction.canceled -= StoppedMovingKey;
    }

    private void SprintStartKey(InputAction.CallbackContext obj)
    {
        _playerSprintKeyPressed = true;
    }

    private void SprintEndKey(InputAction.CallbackContext obj)
    {
        _playerSprintKeyPressed = false;
    }


    //Character direction and movement + Deal with raycast results
    void FixedUpdate()
    {
        if (!_playerMoveKeyPressed || !CanMove)
        {
            if (PlayerMoving)
            {
                PlayerMoving = false;
                OnPlayerStopMove?.Invoke();
            }

            if (PlayerSprinting)
            {
                PlayerSprinting = false;
                OnPlayerStopSprint?.Invoke();
            }
            return;
        }


        MoveVec = _moveAction.ReadValue<Vector2>();

        if (MoveVec.Equals(Vector2.zero))
            return;

        //Check if we were stuck trying to move but now can move
        if (!PlayerMoving)
        {
            PlayerMoving = true;
            OnPlayerStartMove?.Invoke();
        }

        if (!PlayerSprinting && _playerSprintKeyPressed)
        {
            PlayerSprinting = true;
            OnPlayerStartSprint?.Invoke();
        }


        //Get Direction Facing
        MoveDirection lastDirection = CurrentFacingDirection;
        if (MoveVec.x != 0)
            CurrentFacingDirection = MoveVec.x > 0 ? MoveDirection.Right : MoveDirection.Left;
        if (lastDirection != CurrentFacingDirection)
            OnPlayerChangeDirection?.Invoke();

        //Store last known raycast direction
        CurrentRaycastDirection = CurrentDirection;

        //Move the player and store their position
        var movements = CurrentSpeed * Time.fixedDeltaTime * MoveVec;
        _rigidbody2D.MovePosition(_rigidbody2D.position + movements);
        PlayerPosition.Value = transform.position;
    }

    public bool StartUsingTool(float toolUseTime)
    {
        if (UsingTool)
            return false;
        UsingTool = true;

        //Stop the player
        PlayerMoving = false;
        MoveVec = Vector2.zero;

        HeldItemHandler.Instance.StartProgress(toolUseTime);

        //Invoke event
        OnStartUseTool?.Invoke();
        return true;
    }
    public void UseTool()
    {
        UsingTool = false;
        OnEndUseTool?.Invoke();
    }

    public void CancelToolUse()
    {
        UsingTool = false;
    }



}
public enum MoveDirection
{
    Up,
    Down,
    Right,
    Left
}


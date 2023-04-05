using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    //Static
    public static PlayerMovementController Instance;

    //Events
    public event Action OnPlayerStartMove;
    public event Action OnPlayerStopMove;

    public event Action OnPlayerStartSprint;
    public event Action OnPlayerStopSprint;

    public event Action OnPlayerChangeDirection;

    //Public Fields
    [field: SerializeField] public bool PlayerSprinting { get; private set; }
    [field: SerializeField] public ModifiableStat PlayerSpeed { get; private set; }
    [field: SerializeField] public bool UsingTool { get; private set; } = false;
    [field: SerializeField] public Vector2 MoveVec { get; private set; } = Vector2.zero;
    public bool IsWalking { get { return (!PlayerSprinting && (MoveVec.y != 0 || MoveVec.x != 0)); } }
    public MoveDirection CurrentFacingDirection { get; private set; } = MoveDirection.Right;
    public MoveDirection CurrentRaycastDirection { get; private set; } = MoveDirection.Right;

    public bool PlayerWantsToBeMovingButCant { get; private set; }
    public bool PlayerMoving { get { return !MoveVec.Equals(Vector2.zero); } }

    //Local Fields
    [SerializeField] float _baseSpeed;
    [SerializeField] float _runSpeedModifier = 1.5f;
    [SerializeField] public Vector3Variable PlayerPosition;
    [SerializeField] BoolListVariable UIOpen;
    [SerializeField] Rigidbody2D _rigidbody2D;

    private MoveDirection _lastDirection = MoveDirection.Right;
    bool _playerMoveKeyPressed;
    bool _playerSprintKeyPressed;

    //Controls
    private PlayerInput playerInput;
    private InputAction _moveAction;
    private InputAction _sprintAction;

    //Helpers
    private float CurrentSpeed { get { return PlayerSprinting ? PlayerSpeed.CurrentValue * _runSpeedModifier : PlayerSpeed.CurrentValue; } }
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
            return !UIOpen.TrueValueExists() && !NewPlayerAnimationController.Instance.AnimationLocked;
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


        //Player Input
        playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"];
        _sprintAction = playerInput.actions["Sprint"];

    }

    private void Start()
    {
        PlayerPosition.Value = _rigidbody2D.transform.position;
        GlobalSceneManager.OnLocationChange += LocationChange;
    }
    private void LocationChange(WorldLocationData location)
    {
        if (!location.TryGetTransitionWaypoint(out WorldWayPointData waypoint))
            return;

        var pos = waypoint.WayPointWorldPosition;
        Debug.Log("x:" + pos.x + " y:" + pos.y + " z:" + pos.z);

        _rigidbody2D.position = pos;
        PlayerPosition.Value = _rigidbody2D.transform.position;
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
        if (PlayerMoving)
        {
            OnPlayerStopMove?.Invoke();
        }
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
        //If we are no longer pressing the move key or are forced to stop moving, call the events
        if (!_playerMoveKeyPressed || !CanMove)
        {
            return;
        }


        MoveVec = _moveAction.ReadValue<Vector2>();

        if (MoveVec.Equals(Vector2.zero))
            return;

        OnPlayerStartMove?.Invoke();

        if (!PlayerSprinting && _playerSprintKeyPressed)
        {
            PlayerSprinting = true;
            OnPlayerStartSprint?.Invoke();
        }

        if (PlayerSprinting && !_playerSprintKeyPressed)
        {
            PlayerSprinting = false;
            OnPlayerStopSprint?.Invoke();
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
        PlayerPosition.Value = _rigidbody2D.transform.position;
    }

    public bool StartUsingTool(float toolUseTime)
    {
        if (!NewPlayerAnimationController.Instance.TryStartUsingTool())
            return false;

        //Stop the player
        MoveVec = Vector2.zero;
        return true;
    }






}


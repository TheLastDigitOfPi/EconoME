using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    [SerializeField] float speed;
    [SerializeField] Vector3Variable PlayerPosition;
    [SerializeField] BoolListVariable UIOpen;
    [SerializeField] PlayerInventoryManager _inventoryManager;

    public bool UsingTool = false;
    public event Action onUseTool;

    public bool _isRunning { get; private set; }
    public bool _isWalking { get; private set; }
    public PlayerInventoryManager InventoryManager => _inventoryManager;
    public Vector2 MoveVec { get; private set; } = Vector2.zero;
    public MoveDirection Direction { get; private set; } = MoveDirection.Right;


    Rigidbody2D _rigidbody2D;


    public enum MoveDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    public void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    //Character direction and movement + Deal with raycast results
    void FixedUpdate()
    {
        if (!UIOpen.TrueValueExists())
        {
            Vector2 movements = new Vector2(MoveVec.x, MoveVec.y).normalized;
            movements = movements * speed * Time.fixedDeltaTime;
            _rigidbody2D.MovePosition(_rigidbody2D.position + movements);
        }
        PlayerPosition.Value = transform.position;
    }

    private void Update()
    {
        if (!UsingTool)
        {
            MoveVec = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            GetDirection();
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed *= 1.5f;
            }
            _isRunning = true;

        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isRunning = false;
            speed /= 1.5f;
        }
    }

    public void StartUsingTool()
    {
        UsingTool = true;
        MoveVec = Vector2.zero;
    }
    public void UseTool()
    {
        onUseTool?.Invoke();
        onUseTool = null;
        UsingTool = false;
    }

    public void CancelToolUse()
    {
        onUseTool = null;
        UsingTool = false;
    }

    private void GetDirection()
    {
        if (MoveVec.y > 0) { Direction = MoveDirection.Up; }
        if (MoveVec.y < 0) { Direction = MoveDirection.Down; }
        if (MoveVec.x < 0) { Direction = MoveDirection.Left; }
        if (MoveVec.x > 0) { Direction = MoveDirection.Right; }

        _isWalking = !_isRunning && (MoveVec.y != 0 || MoveVec.x != 0) ? true : false;
    }


}



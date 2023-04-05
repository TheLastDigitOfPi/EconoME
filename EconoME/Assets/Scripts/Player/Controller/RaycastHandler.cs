using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RaycastHandler : MonoBehaviour
{
    //Static
    public static RaycastHandler Instance;
    //Events
    public event Action<IAmInteractable> OnRaycastSuccess;
    public event Action OnRaycastFail;
    //Public fields

    //Local Serialized fields

    [SerializeField] BoolListVariable UIOpen;
    [SerializeField] Vector3Variable playerPos;
    [SerializeField] float _raycastLength = 1f;

    //Local Private fields
    Vector3 raycastOffSet = new Vector3(0, 0.4f);
    PlayerMovementController owner;
    [SerializeField] PlayerInput playerInput;
    private InputAction _interactAction;
    bool _playerTryingToRaycast;

    //Helpers
    bool CanRayCast { get { return !UIOpen.TrueValueExists() && !owner.UsingTool; } }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 raycast handler found");
            Destroy(this);
            return;
        }
        Instance = this;
        _interactAction = playerInput.actions["Interact"];
    }

    private void Start()
    {
        owner = PlayerMovementController.Instance;
    }

    private void OnEnable()
    {
        _interactAction.performed += StartAttemptRaycast;
        _interactAction.canceled += StopTryRaycast;
    }

    private void StopTryRaycast(InputAction.CallbackContext obj)
    {
        _playerTryingToRaycast = false;
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        _interactAction.performed -= StartAttemptRaycast;
        _interactAction.canceled -= StopTryRaycast;
    }
    void StartAttemptRaycast(InputAction.CallbackContext obj)
    {
        _playerTryingToRaycast = true;

        StartCoroutine(RaycastInterval());

        IEnumerator RaycastInterval()
        {
            while (_playerTryingToRaycast)
            {
                if (CanRayCast)
                {
                    RaycastAndNotifiyPotentialHits();
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    public void RaycastAndNotifiyPotentialHits()
    {
        RaycastHit2D[] raycastData = getHit();
        if (raycastData == null) { OnRaycastFail?.Invoke(); return; }

        for (int i = 0; i < raycastData.Length; i++)
        {
            if (raycastData[i].collider.TryGetComponent(out IAmInteractable target))
            {
                if (!target.OnRaycastHit(owner, raycastData[i].collider))
                    continue;
                OnRaycastSuccess?.Invoke(target);
                return;
            }
        }
        OnRaycastFail?.Invoke();
    }

    RaycastHit2D[] getHit()
    {
        Vector2 direction = owner.MoveVec;
        if (direction == Vector2.zero)
            switch (owner.CurrentRaycastDirection)
            {
                case MoveDirection.Up:
                    direction = Vector2.up;
                    break;
                case MoveDirection.Down:
                    direction = Vector2.down;
                    break;
                case MoveDirection.Right:
                    direction = Vector2.right;
                    break;
                case MoveDirection.Left:
                    direction = Vector2.left;
                    break;
                default:
                    direction = Vector2.zero;
                    break;
            }

        var dirLength = _raycastLength;
        if(owner.CurrentRaycastDirection == MoveDirection.Up || owner.CurrentRaycastDirection == MoveDirection.Down)
            dirLength /=2;
        Debug.DrawRay(playerPos.Value + raycastOffSet, direction.normalized * dirLength, Color.red, 3f);
        return Physics2D.RaycastAll(playerPos.Value + raycastOffSet, direction, dirLength);
    }
}

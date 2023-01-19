using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastHandler : MonoBehaviour
{

    [SerializeField] BoolListVariable UIOpen;
    [SerializeField] Vector3Variable playerPos;

    PlayerController owner;
    private void Awake()
    {
        owner = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIOpen.TrueValueExists())
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DealWtihRayCastResult();
            }
        }
    }
    public void DealWtihRayCastResult()
    {
        RaycastHit2D[] raycastData = getHit();
        if (raycastData == null) { return; }

        for (int i = 0; i < raycastData.Length; i++)
        {
            if (raycastData[i].collider.TryGetComponent(out Raycastable target))
            {
                if (target.OnRaycastHit(owner, raycastData[i].collider))
                    return;
            }
        }


    }

    RaycastHit2D[] getHit()
    {
        Vector2 direction = owner.MoveVec;
        if (direction == Vector2.zero)
            switch (owner.Direction)
            {
                case PlayerController.MoveDirection.Up:
                    direction = Vector2.up;
                    break;
                case PlayerController.MoveDirection.Down:
                    direction = Vector2.down;
                    break;
                case PlayerController.MoveDirection.Right:
                    direction = Vector2.right;
                    break;
                case PlayerController.MoveDirection.Left:
                    direction = Vector2.left;
                    break;
                default:
                    direction = Vector2.zero;
                    break;
            }
        Debug.DrawRay(playerPos.Value, direction.normalized * 3f, Color.red, 3f);
        return Physics2D.RaycastAll(playerPos.Value, direction, 3f);

    }
}

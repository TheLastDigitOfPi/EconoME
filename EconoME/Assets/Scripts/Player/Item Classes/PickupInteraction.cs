using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteraction : MonoBehaviour
{
    //Script for moving ground items towards player
    //Player collider script starts the movement, if too far away stop movement
    public bool CanBePickedUp = true;
    //SemiPublic Data
    public bool moving = false;
    //Private Data
    Rigidbody2D rb;

    WorldItemHandler handler;
    Vector3 playerPos { get { return PlayerMovementController.Instance.PlayerPosition.Value; } }

    void Awake()
    {
        handler = GetComponent<WorldItemHandler>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (moving)
        {
            // movetoPlayer();
        }
    }

    void WaitForPickup()
    {
        StartCoroutine(PickupDelay());
    }

    IEnumerator PickupDelay()
    {
        yield return new WaitForSeconds(3f);
        CanBePickedUp = true;
    }

    //Moves towards player by adding force to rigidbody
    void movetoPlayer()
    {
        if ((transform.position - playerPos).magnitude > 7)
        {
            rb.velocity = Vector3.zero;
            moving = false;
            return;
        }
        if (transform.position.x > playerPos.x)
        {
            rb.AddForce(new Vector2(-5, 0));
            if (rb.velocity.x > 20)
            {
                rb.AddForce(new Vector2(-40, 0));
            }
        }
        if (transform.position.x < playerPos.x)
        {
            rb.AddForce(new Vector2(5, 0));
            if (rb.velocity.x < -20)
            {
                rb.AddForce(new Vector2(40, 0));
            }
        }
        if (transform.position.y > playerPos.y)
        {
            rb.AddForce(new Vector2(0, -5));
            if (rb.velocity.y > 20)
            {
                rb.AddForce(new Vector2(0, -40));
            }
        }
        if (transform.position.y < playerPos.y)
        {
            rb.AddForce(new Vector2(0, 5));
            if (rb.velocity.y < -20)
            {
                rb.AddForce(new Vector2(0, 40));
            }
        }

    }

    public void startMovetoPlayer()
    {
        moving = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (CanBePickedUp && !handler.SetForRemoval)
        {
            if (collision.CompareTag("Player"))
            {
                if (PlayerInventoryManager.AddItemToPlayer(handler.data.item))
                {
                    GroundItemManager.Instance.ItemRemoved(handler);
                    handler.PickupByPlayer = true;
                }

                handler.UpdateText();
                return;
            }
        }

    }

    public void CheckForPlayerNear()
    {
        if ((transform.position - playerPos).magnitude < 5)
        {
            startMovetoPlayer();
        }
    }
}

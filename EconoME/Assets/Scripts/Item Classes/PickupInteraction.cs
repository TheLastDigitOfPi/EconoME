using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupInteraction : MonoBehaviour
{
    //Script for moving ground items towards player
    //Player collider script starts the movement, if too far away stop movement
    public bool CanBePickedUp = true;
    //SemiPublic Data
    Transform player;
    public bool moving = false;
    //Private Data
    Rigidbody2D rb;


    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (moving)
        {
            movetoPlayer();
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
        if ((transform.position - player.position).magnitude > 7)
        {
            rb.velocity = Vector3.zero;
            moving = false;
            return;
        }
        if (transform.position.x > player.position.x)
        {
            rb.AddForce(new Vector2(-5, 0));
            if (rb.velocity.x > 20)
            {
                rb.AddForce(new Vector2(-40, 0));
            }
        }
        if (transform.position.x < player.position.x)
        {
            rb.AddForce(new Vector2(5, 0));
            if (rb.velocity.x < -20)
            {
                rb.AddForce(new Vector2(40, 0));
            }
        }
        if (transform.position.y > player.position.y)
        {
            rb.AddForce(new Vector2(0, -5));
            if (rb.velocity.y > 20)
            {
                rb.AddForce(new Vector2(0, -40));
            }
        }
        if (transform.position.y < player.position.y)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CanBePickedUp)
        {
            if (collision.CompareTag("Player"))
            {
                var handler = gameObject.GetComponent<WorldItemHandler>();
                PlayerController owner = collision.GetComponent<PlayerController>();
                
                if(owner.InventoryManager.AddItemToPlayer(handler.data.item)){handler.DestroyItem();}
                handler.updateText();
                return;
            }
        }

    }

    public void CheckForPlayerNear()
    {
        if((transform.position - player.transform.position).magnitude < 5)
        {
            startMovetoPlayer();
        }
    }
}

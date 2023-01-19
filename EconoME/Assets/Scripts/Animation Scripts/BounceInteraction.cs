using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Placed on each item that can bounce.
/*
TODO - This script should be placed on the object when it needs to bounce
and be removed after finishing the bounce(s). Can remake the class to handle
more object movements such as pushes and pulls.
 */
public class BounceInteraction : MonoBehaviour
{
    Rigidbody2D rb;

    float groundPos;
    public int Maxbounces = 5;
    int RemainingBounces;
    public float Gravity = 1f;
    bool Bouncing = false;
     PickupInteraction pickupInteraction;
    void Bounce()
    {
        rb.gravityScale =  Gravity;
        Vector2 forceup = new Vector2(0, 130 * 1/2 * (RemainingBounces));
        rb.AddForce(forceup);

        RemainingBounces--;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        RemainingBounces = Maxbounces;
        pickupInteraction = GetComponent<PickupInteraction>();
    }

    private void FixedUpdate()
    {
        if (Bouncing)
        {
            if (transform.localPosition.y < groundPos)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                transform.localPosition = new Vector3(transform.position.x, groundPos, transform.position.z);
                if (RemainingBounces > 0)
                {
                    Bounce();
                    return;
                }
                rb.velocity = new Vector2(0, 0);
                Bouncing = false;
                rb.gravityScale = 0;
                pickupInteraction.CanBePickedUp = true;
                pickupInteraction.CheckForPlayerNear();

            }
        }

    }
    [ContextMenu("Bounce")]
    public void StartBounce()
    {
        pickupInteraction.CanBePickedUp = false;
        groundPos = transform.position.y;
        RemainingBounces = Maxbounces;
        Bouncing = true;
        float xDirection = Random.Range(-150, 150);
        rb.AddForce(new Vector2(xDirection, 0));
        Bounce();
    }
}

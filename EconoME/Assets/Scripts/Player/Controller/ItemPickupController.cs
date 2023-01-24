using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupController : MonoBehaviour
{
    //Script placed on Character/Character child to initiate collection of ground items

    //Private Data
    [SerializeField]
    float size;
    CircleCollider2D colliders;

    private void Start()
    {
        colliders = GetComponent<CircleCollider2D>();
        colliders.radius = size;
    }

    //If interacting with Pickup item, start item moving to player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name + " was hit");

        if (collision.gameObject.CompareTag("Pickup"))
        {
            PickupInteraction PI = collision.gameObject.GetComponent<PickupInteraction>();
            if (PI.CanBePickedUp)
            {
                PI.startMovetoPlayer();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickup"))
        {
            PickupInteraction PI = collision.gameObject.GetComponent<PickupInteraction>();
            if (PI.CanBePickedUp)
            {
                PI.startMovetoPlayer();
            }
        }
    }
    //setter
    public void setSize(float s)
    {
        if (s > 0)
        {
            size = s;
        }

    }
}

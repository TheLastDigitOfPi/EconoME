using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Placed on each item that can bounce.
/*
TODO - This script should be placed on the object when it needs to bounce
and be removed after finishing the bounce(s). Can remake the class to handle
more object movements such as pushes and pulls.
 */
public class BounceInteraction : MonoBehaviour
{


    [SerializeField] Vector2 XMoveAmountRange;
    [SerializeField] Vector2 YMoveAmountRange;
    [SerializeField] Vector2Int NumberOfJumpsRange;
    [SerializeField] Vector2 JumpHeightRange;
    [SerializeField] float TimeBouncing;

    PickupInteraction pickupInteraction;
    WorldItemHandler handler;

    private void Awake()
    {
        handler = GetComponent<WorldItemHandler>();
        pickupInteraction = GetComponent<PickupInteraction>();
    }



    private void FixedUpdate()
    {
        /*
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
        */

    }
    [ContextMenu("Bounce")]
    public void StartBounce()
    {
        pickupInteraction.CanBePickedUp = false;
        float xMoveAmount = Random.Range(XMoveAmountRange.x, XMoveAmountRange.y);
        float yMoveAmount = Random.Range(YMoveAmountRange.x, YMoveAmountRange.y);


        float JumpHeight = Random.Range(JumpHeightRange.x, JumpHeightRange.y);
        int NumberOfJumps = Random.Range(NumberOfJumpsRange.x, NumberOfJumpsRange.y);

        var startingPos = transform.localPosition;
        Vector3 endPos = new Vector3(startingPos.x + (xMoveAmount * (1f / NumberOfJumps)), startingPos.y + yMoveAmount * (1f / NumberOfJumps), startingPos.z);
        int bouncesCompleted = 0;

        transform.DOLocalJump(endPos, JumpHeight, 1, TimeBouncing / NumberOfJumps).SetEase(Ease.Linear).onComplete += () =>
        {
            bounce();
        };
        void bounce()
        {
            bouncesCompleted++;
            if (bouncesCompleted >= NumberOfJumps)
            {
                pickupInteraction.CanBePickedUp = true;
                pickupInteraction.CheckForPlayerNear();
                return;
            }

            int currentBounceNum = bouncesCompleted + 1;
            float bouncePercent = (float)currentBounceNum / NumberOfJumps;
            float currentJumpHeight = (float)JumpHeight / currentBounceNum;
            float currentXMovePos = startingPos.x + (xMoveAmount * bouncePercent);
            float currentYMovePos = startingPos.y + (yMoveAmount * bouncePercent);

            Vector3 currentEndPos = new Vector3(currentXMovePos, currentYMovePos, startingPos.z);
            transform.DOLocalJump(currentEndPos, currentJumpHeight, 1, TimeBouncing / NumberOfJumps).SetEase(Ease.Linear).onComplete += () => { bounce(); };
        }



        /*
        groundPos = transform.position.y;
        RemainingBounces = Maxbounces;
        Bouncing = true;
        float xDirection = Random.Range(-150, 150);
        rb.AddForce(new Vector2(xDirection, 0));
        Bounce();
        */
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}

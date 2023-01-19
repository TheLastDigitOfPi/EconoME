using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Calls animator to animate brush when player, npc, animal, etc
 * collides with it.
 * 
 */
public class AnimateBrush : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Play Animation when something collides with it
        animator.enabled = true;
        if (other.CompareTag("Player") || other.CompareTag("Creature"))
        {
            animator.SetTrigger("Animate");
            StartCoroutine(PlayAnimation());
        }
    }

    IEnumerator PlayAnimation()
    {
        yield return new WaitForSeconds(1f);
        animator.enabled = false;
    }
}

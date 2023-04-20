using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public static class UnityCanvasExtensions
{
    public static void ToggleCanvas(this CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = canvasGroup.alpha == 1 ? 0 : 1;
        canvasGroup.interactable = !canvasGroup.interactable;
        canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
    }

    public static void SetCanvas(this CanvasGroup canvasGroup, bool CanvasEnabled)
    {
        canvasGroup.alpha = CanvasEnabled ? 1 : 0;
        canvasGroup.interactable = CanvasEnabled;
        canvasGroup.blocksRaycasts = CanvasEnabled;
    }
}

public static class CombatExtensions
{
    public static void DoKnockback(MonoBehaviour script, NavMeshAgent _agent, Rigidbody2D _rigidbody, float distance, Vector2 direction)
    {
        if (_agent != null)
        {
            _agent.enabled = false;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.isKinematic = false;
            _rigidbody.velocity = direction * distance;
            script.StartCoroutine(WaitForKnockback());
            IEnumerator WaitForKnockback()
            {
                while (_rigidbody.velocity.magnitude > 0.15f)
                {
                    yield return null;
                }
                _rigidbody.velocity = Vector2.zero;
                _rigidbody.bodyType = RigidbodyType2D.Kinematic;
                _rigidbody.isKinematic = true;
                _agent.enabled = true;
            }
        }
    }
}
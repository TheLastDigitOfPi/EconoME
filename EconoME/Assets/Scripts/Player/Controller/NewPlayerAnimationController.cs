using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerAnimationController : MonoBehaviour
{
    [Header("Requirements")]
    [Space(10)]
    [SerializeField] Animator _animator;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [Space(10)]
    [Header("Animations")]
    [SerializeField] string _movementAnimationName = "Movement";
    [SerializeField] string _hurtAnimationName = "Hurt";
    [SerializeField] string _idleAnimationName = "Idle";
    [SerializeField] string _deathAnimationName = "Die";
    [SerializeField] string _interactAnimationName = "Wave";

    private void Start()
    {
        PlayerMovementController.Instance.OnPlayerStartMove += StartMoving;
        PlayerMovementController.Instance.OnPlayerStopMove += StopMoving;
        PlayerMovementController.Instance.OnPlayerChangeDirection += ChangeDirection;
        PlayerMovementController.Instance.OnPlayerStartSprint += StartSprinting;
        PlayerMovementController.Instance.OnPlayerStopSprint += StopSprinting;
        PlayerCombatController.Instance.OnPlayerDeath += PlayerDeath;
        PlayerMovementController.Instance.OnStartUseTool += StartUsingTool;
        PlayerMovementController.Instance.OnEndUseTool += StopUsingTool;
    }

    private void StopUsingTool()
    {
        StopAllCoroutines();
        StartCoroutine(WaitForNextAction());
    }

    IEnumerator WaitForNextAction()
    {
        yield return new WaitForSeconds(0.5f);
        if (!PlayerMovementController.Instance.PlayerMoving)
            StopMoving();
    }
    private void StartUsingTool()
    {
        StopAllCoroutines();
        var currentClipInfo = _animator.GetCurrentAnimatorClipInfo(0);
        if (currentClipInfo[0].clip.name != _interactAnimationName)
            _animator.CrossFade(_interactAnimationName, 0);
    }

    private void StopSprinting()
    {
        _animator.speed = 1f;
    }

    private void StartSprinting()
    {
        _animator.speed = 1.5f;
    }

    private void ChangeDirection()
    {
        _spriteRenderer.flipX = PlayerMovementController.Instance.CurrentDirection == MoveDirection.Left;
    }

    private void StopMoving()
    {
        _animator.CrossFade(_idleAnimationName, 0);
    }

    private void OnDestroy()
    {
        if (PlayerMovementController.Instance != null)
        {
            PlayerMovementController.Instance.OnPlayerStartMove -= StartMoving;
            PlayerMovementController.Instance.OnPlayerStopMove -= StopMoving;
            PlayerMovementController.Instance.OnPlayerChangeDirection -= ChangeDirection;
            PlayerMovementController.Instance.OnPlayerStartSprint -= StartSprinting;
            PlayerMovementController.Instance.OnPlayerStopSprint -= StopSprinting;
        }
        if (PlayerCombatController.Instance != null)
        {
            PlayerCombatController.Instance.OnPlayerDeath -= PlayerDeath;
        }
    }

    private void PlayerDeath()
    {
        _animator.CrossFade(_deathAnimationName, 0);
    }

    private void StartMoving()
    {
        if (PlayerMovementController.Instance.UsingTool)
            return;
        StartCoroutine(MoveAnimation());
        IEnumerator MoveAnimation()
        {
            yield return new WaitUntil(() => { return !PlayerMovementController.Instance.MoveVec.Equals(Vector2.zero); });
            _spriteRenderer.flipX = PlayerMovementController.Instance.CurrentFacingDirection == MoveDirection.Left;
            if (PlayerMovementController.Instance.PlayerSprinting)
                StartSprinting();
            _animator.CrossFade(_movementAnimationName, 0);
        }

    }
}

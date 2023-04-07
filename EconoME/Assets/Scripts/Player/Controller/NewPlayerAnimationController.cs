using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerAnimationController : MonoBehaviour
{
    public static NewPlayerAnimationController Instance { get; private set; }

    [field: SerializeField] public bool AnimationLocked { get; private set; }
    public bool UsingTool { get; private set; }
    [Header("Requirements")]
    [Space(10)]
    [SerializeField] Animator _animator;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [Space(10)]
    [Header("Animations")]
    [SerializeField] string _movementAnimationName = "Movement";
    //[SerializeField] string _hurtAnimationName = "Hurt";
    [SerializeField] string _idleAnimationName = "Idle";
    [SerializeField] string _deathAnimationName = "Die";
    [SerializeField] string _interactAnimationName = "Wave";
    [SerializeField] string _swordAttack1AnimationName = "Swing Sword Down";
    [SerializeField] string _swordAttack2AnimationName = "Swing Sword Forward";
    [SerializeField] string _swordAttack3AnimationName = "Swing Sword Up";
    [SerializeField] string _shootBowAnimationName = "Shoot Bow";

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 Player animation Controller found");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlayerMovementController.Instance.OnPlayerStartMove += StartMoving;
        PlayerMovementController.Instance.OnPlayerStopMove += StopMoving;
        PlayerMovementController.Instance.OnPlayerChangeDirection += ChangeDirection;
        PlayerMovementController.Instance.OnPlayerStartSprint += StartSprinting;
        PlayerMovementController.Instance.OnPlayerStopSprint += StopSprinting;
        PlayerCombatController.Instance.OnDeath += PlayerDeath;
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
            PlayerCombatController.Instance.OnDeath -= PlayerDeath;
        }
    }

    internal float GetAnimationTime(AttackAnimation attackAnimation)
    {
        return attackAnimation switch
        {
            AttackAnimation.SwingDown => ClipAnimationTime(_swordAttack1AnimationName),
            AttackAnimation.SwingForward => ClipAnimationTime(_swordAttack2AnimationName),
            AttackAnimation.SwingUp => ClipAnimationTime(_swordAttack3AnimationName),
            AttackAnimation.ShootBow => ClipAnimationTime(_shootBowAnimationName),
            _ => ClipAnimationTime(_swordAttack1AnimationName)
        };
        float ClipAnimationTime(string clipName)
        {
            var controller = _animator.runtimeAnimatorController;
            var clip = controller.animationClips.First(a => a.name == clipName);
            return clip.length;
        }
    }

    void StopAnimation()
    {
        StopAllCoroutines();
        AnimationLocked = false;
        _animator.speed = 1;
    }
    public bool TryPlayAttackAnimation(AttackAnimation attackAnim, float attackSpeed)
    {
        if (AnimationLocked)
            return false;
        StopAnimation();
        AnimationLocked = true;
        string attackName = GetAttackName(attackAnim);
        StartCoroutine(PlayAttack());
        return true;

        IEnumerator PlayAttack()
        {
            //Flip view to side mouse is on
            //_spriteRenderer.flipX = Camera.main.ScreenToWorldPoint(Mouse.current.position.value).x < PlayerMovementController.Instance.PlayerPosition.Value.x;
            _animator.speed = attackSpeed;
            _animator.CrossFade(attackName, 0);
            yield return new WaitForSeconds((GetAnimationTime(attackAnim) / attackSpeed) * 0.5f);
            AnimationLocked = false;
            yield return new WaitForSeconds((GetAnimationTime(attackAnim) / attackSpeed) * 0.5f);
            _animator.speed = 1;
            Idle();
        }

        string GetAttackName(AttackAnimation animation)
        {

            int randAnim = UnityEngine.Random.Range(0, 3);

            return randAnim switch
            {
                0 => _swordAttack1AnimationName,
                1 => _swordAttack2AnimationName,
                2 => _swordAttack3AnimationName,
                _ => _swordAttack1AnimationName
            };

            return animation switch
            {
                AttackAnimation.SwingDown => _swordAttack1AnimationName,
                AttackAnimation.SwingForward => _swordAttack2AnimationName,
                AttackAnimation.SwingUp => _swordAttack3AnimationName,
                AttackAnimation.ShootBow => _shootBowAnimationName,
                _ => _swordAttack1AnimationName,
            };
        }
    }
    Coroutine _toolWaitingCoroutine;
    public bool TryStartUsingTool()
    {
        //If we are already in a locked animation, fail the action
        if (AnimationLocked)
            return false;

        Debug.Log("Starting Tool Anim");
        if (_toolWaitingCoroutine != null)
        {
            StopCoroutine(_toolWaitingCoroutine);
            _toolWaitingCoroutine = null;
        }

        AnimationLocked = true;
        _animator.CrossFade(_interactAnimationName, 0);

        //Subscribe to when to stop the animation
        HeldItemHandler.Instance.OnComplete += ToolComplete;
        return true;
        void ToolComplete()
        {
            AnimationLocked = false;
            HeldItemHandler.Instance.OnComplete -= ToolComplete;
            //Stop the animation after a set time, cancel the routine if using tool again
            _toolWaitingCoroutine = StartCoroutine(WaitForNextAction());
            IEnumerator WaitForNextAction()
            {
                //Wait some time
                yield return new WaitForSeconds(0.3f);
                //If our animation has not been changed by anyone and the coroutine hasn't been stopped, we will idle
                var currentClipInfo = _animator.GetCurrentAnimatorClipInfo(0);
                Debug.Log("At Stop Idle");
                if (currentClipInfo[0].clip.name == _interactAnimationName)
                    Idle();
                _toolWaitingCoroutine = null;
            }
        }
    }

    private void ChangeDirection()
    {
        _spriteRenderer.flipX = PlayerMovementController.Instance.CurrentDirection == MoveDirection.Left;
    }

    void Idle()
    {
        _animator.CrossFade(_idleAnimationName, 0);
    }
    private void PlayerDeath()
    {
        _animator.CrossFade(_deathAnimationName, 0);
    }

    Coroutine _waitForMove;
    private void StopMoving()
    {
        if (AnimationLocked || _toolWaitingCoroutine != null)
            return;
        StopAnimation();
        _animator.CrossFade(_idleAnimationName, 0);
    }


    private void StartMoving()
    {
        if (AnimationLocked)
            return;
        _spriteRenderer.flipX = PlayerMovementController.Instance.CurrentFacingDirection == MoveDirection.Left;

        Debug.Log("Started Moving");
        _animator.CrossFade(_movementAnimationName, 0);

    }
    private void StopSprinting()
    {
        _animator.speed = 1f;
    }

    private void StartSprinting()
    {
        _animator.speed = 1.5f;
    }

    public enum AttackAnimation
    {
        SwingDown,
        SwingForward,
        SwingUp,
        ShootBow
    }
}

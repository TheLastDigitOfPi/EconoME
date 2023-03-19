using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerAnimationController : MonoBehaviour
{

    [SerializeField] ParticleSystem DropInBurst;
    [SerializeField] ParticleSystem WalkingEffect;

    [SerializeField] SpriteRenderer _baseRenderer;
    [SerializeField] SpriteRenderer _hairRenderer;
    [SerializeField] SpriteRenderer _shirtRenderer;
    [SerializeField] SpriteRenderer _pantsRenderer;
    [SerializeField] SpriteRenderer _shoesRenderer;
    [SerializeField] SpriteRenderer _toolRenderer;


    bool _isRunning => _playerController.PlayerSprinting;
    bool _isWalking => _playerController.IsWalking;
    MoveDirection direction => _playerController.CurrentDirection;

    float constantEmission;

    [SerializeField] CharacterAnimationSet WalkLeftSet;
    [SerializeField] CharacterAnimationSet WalkRightSet;
    [SerializeField] CharacterAnimationSet WalkUpSet;
    [SerializeField] CharacterAnimationSet WalkDownSet;

    [SerializeField] CharacterAnimationSet RunLeftSet;
    [SerializeField] CharacterAnimationSet RunRightSet;
    [SerializeField] CharacterAnimationSet RunUpSet;
    [SerializeField] CharacterAnimationSet RunDownSet;

    [SerializeField] CharacterAnimationSet IdleLeftSet;
    [SerializeField] CharacterAnimationSet IdleRightSet;
    [SerializeField] CharacterAnimationSet IdleUpSet;
    [SerializeField] CharacterAnimationSet IdleDownSet;

    [SerializeField] CharacterAnimationSet UseToolLeftSet;
    [SerializeField] CharacterAnimationSet UseToolRightSet;
    [SerializeField] CharacterAnimationSet UseToolUpSet;
    [SerializeField] CharacterAnimationSet UseToolDownSet;

    StateMachine _stateMachine;
    PlayerMovementController _playerController;

    private void Awake()
    {
        GameLoopEvents.gameStarted = null;

        SpriteRenderer[] AllRenderers = new SpriteRenderer[5];
        AllRenderers[0] = _baseRenderer;
        AllRenderers[1] = _hairRenderer;
        AllRenderers[2] = _shirtRenderer;
        AllRenderers[3] = _pantsRenderer;
        AllRenderers[4] = _shoesRenderer;

        _stateMachine = new();
        _playerController = GetComponentInParent<PlayerMovementController>();

        var WalkLeft = new AnimateSprite(WalkLeftSet, AllRenderers);
        var WalkRight = new AnimateSprite(WalkRightSet, AllRenderers);
        var WalkUp = new AnimateSprite(WalkUpSet, AllRenderers);
        var WalkDown = new AnimateSprite(WalkDownSet, AllRenderers);

        var RunLeft = new AnimateSprite(RunLeftSet, AllRenderers);
        var RunRight = new AnimateSprite(RunRightSet, AllRenderers);
        var RunDown = new AnimateSprite(RunDownSet, AllRenderers);
        var RunUp = new AnimateSprite(RunUpSet, AllRenderers);

        var IdleLeft = new AnimateSprite(IdleLeftSet, AllRenderers);
        var IdleRight = new AnimateSprite(IdleRightSet, AllRenderers);
        var IdleUp = new AnimateSprite(IdleUpSet, AllRenderers);
        var IdleDown = new AnimateSprite(IdleDownSet, AllRenderers);

        var UseToolLeft = new UseTool(UseToolLeftSet, AllRenderers, _playerController, _toolRenderer);
        var UseToolRight = new UseTool(UseToolRightSet, AllRenderers, _playerController, _toolRenderer);
        var UseToolUp = new UseTool(UseToolUpSet, AllRenderers, _playerController, _toolRenderer);
        var UseToolDown = new UseTool(UseToolDownSet, AllRenderers, _playerController, _toolRenderer);


        _stateMachine.AddAnyTransition(UseToolLeft, IsUsingToolLeft());
        _stateMachine.AddAnyTransition(UseToolRight, IsUsingToolRight());
        _stateMachine.AddAnyTransition(UseToolDown, IsUsingToolDown());
        _stateMachine.AddAnyTransition(UseToolUp, IsUsingToolUp());

        _stateMachine.AddAnyTransition(IdleLeft, IsIdleLeft());
        _stateMachine.AddAnyTransition(IdleRight, IsIdleRight());
        _stateMachine.AddAnyTransition(IdleUp, IsIdleUp());
        _stateMachine.AddAnyTransition(IdleDown, IsIdleDown());

        _stateMachine.AddAnyTransition(WalkDown, IsWalkingDown());
        _stateMachine.AddAnyTransition(WalkUp, IsWalkingUp());
        _stateMachine.AddAnyTransition(WalkRight, IsWalkingRight());
        _stateMachine.AddAnyTransition(WalkLeft, IsWalkingLeft());

        _stateMachine.AddAnyTransition(RunDown, IsRunningDown());
        _stateMachine.AddAnyTransition(RunUp, IsRunningUp());
        _stateMachine.AddAnyTransition(RunRight, IsRunningRight());
        _stateMachine.AddAnyTransition(RunLeft, IsRunningLeft());

        _stateMachine.SetState(IdleRight);

        Func<bool> IsRunningRight() => () => _isRunning && !_isWalking && direction == MoveDirection.Right;
        Func<bool> IsRunningLeft() => () => _isRunning && !_isWalking && direction == MoveDirection.Left;
        Func<bool> IsRunningUp() => () => _isRunning && !_isWalking && direction == MoveDirection.Up;
        Func<bool> IsRunningDown() => () => _isRunning && !_isWalking && direction == MoveDirection.Down;

        Func<bool> IsWalkingRight() => () => _isWalking && !_isRunning && direction == MoveDirection.Right;
        Func<bool> IsWalkingLeft() => () => _isWalking && !_isRunning && direction == MoveDirection.Left;
        Func<bool> IsWalkingUp() => () => _isWalking && !_isRunning && direction == MoveDirection.Up;
        Func<bool> IsWalkingDown() => () => _isWalking && !_isRunning && direction == MoveDirection.Down;


        Func<bool> IsIdleRight() => () => !_isWalking && !_isRunning && direction == MoveDirection.Right;
        Func<bool> IsIdleLeft() => () => !_isWalking && !_isRunning && direction == MoveDirection.Left;
        Func<bool> IsIdleUp() => () => !_isWalking && !_isRunning && direction == MoveDirection.Up;
        Func<bool> IsIdleDown() => () => !_isWalking && !_isRunning && direction == MoveDirection.Down;

        Func<bool> IsUsingToolRight() => () => _playerController.UsingTool && direction == MoveDirection.Right;
        Func<bool> IsUsingToolLeft() => () => _playerController.UsingTool && direction == MoveDirection.Left;
        Func<bool> IsUsingToolUp() => () => _playerController.UsingTool && direction == MoveDirection.Up;
        Func<bool> IsUsingToolDown() => () => _playerController.UsingTool && direction == MoveDirection.Down;

    }
    void Start()
    {
        GameLoopEvents.gameStarted += DropIn;
        constantEmission = WalkingEffect.main.startSizeMultiplier;
    }

    private void OnDisable()
    {
        GameLoopEvents.gameStarted -= DropIn;
        GameLoopEvents.GameStarted = false;
    }

    void DropIn()
    {
        StartCoroutine(PlayerDroppedIn(0.5f));
    }

    IEnumerator PlayerDroppedIn(float animationTime)
    {

        yield return new WaitForSeconds(animationTime);
        GameLoopEvents.GameStarted = true;
        GameLoopEvents.gameStarted = null;
        DropInBurst.Play();
    }

    private void FixedUpdate()
    {
        _stateMachine.Tick();
    }

}

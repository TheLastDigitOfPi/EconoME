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


    bool _isRunning => _playerController._isRunning;
    bool _isWalking => _playerController._isWalking;
    PlayerController.MoveDirection direction => _playerController.Direction;

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
    PlayerController _playerController;

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
        _playerController = GetComponentInParent<PlayerController>();

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

        Func<bool> IsRunningRight() => () => _isRunning && !_isWalking && direction == PlayerController.MoveDirection.Right;
        Func<bool> IsRunningLeft() => () => _isRunning && !_isWalking && direction == PlayerController.MoveDirection.Left;
        Func<bool> IsRunningUp() => () => _isRunning && !_isWalking && direction == PlayerController.MoveDirection.Up;
        Func<bool> IsRunningDown() => () => _isRunning && !_isWalking && direction == PlayerController.MoveDirection.Down;

        Func<bool> IsWalkingRight() => () => _isWalking && !_isRunning && direction == PlayerController.MoveDirection.Right;
        Func<bool> IsWalkingLeft() => () => _isWalking && !_isRunning && direction == PlayerController.MoveDirection.Left;
        Func<bool> IsWalkingUp() => () => _isWalking && !_isRunning && direction == PlayerController.MoveDirection.Up;
        Func<bool> IsWalkingDown() => () => _isWalking && !_isRunning && direction == PlayerController.MoveDirection.Down;


        Func<bool> IsIdleRight() => () => !_isWalking && !_isRunning && direction == PlayerController.MoveDirection.Right;
        Func<bool> IsIdleLeft() => () => !_isWalking && !_isRunning && direction == PlayerController.MoveDirection.Left;
        Func<bool> IsIdleUp() => () => !_isWalking && !_isRunning && direction == PlayerController.MoveDirection.Up;
        Func<bool> IsIdleDown() => () => !_isWalking && !_isRunning && direction == PlayerController.MoveDirection.Down;

        Func<bool> IsUsingToolRight() => () => _playerController.UsingTool && direction == PlayerController.MoveDirection.Right;
        Func<bool> IsUsingToolLeft() => () => _playerController.UsingTool && direction == PlayerController.MoveDirection.Left;
        Func<bool> IsUsingToolUp() => () => _playerController.UsingTool && direction == PlayerController.MoveDirection.Up;
        Func<bool> IsUsingToolDown() => () => _playerController.UsingTool && direction == PlayerController.MoveDirection.Down;

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

public class AnimateSprite : IState
{
    protected readonly CharacterAnimationSet animationSet;
    protected readonly SpriteRenderer[] renderers;
    protected float TimePerFrame { get { return animationSet.SecondersPerFrame; } }
    protected float CurrentFrameTime = 0f;

    public AnimateSprite(CharacterAnimationSet animationSet, SpriteRenderer[] renderers)
    {
        this.animationSet = animationSet;
        this.renderers = renderers;
        animationSet.ResetAll();
    }

    public virtual void OnEnter()
    {
        CurrentFrameTime = TimePerFrame;
    }

    public virtual void OnExit()
    {
    }

    public virtual void Tick()
    {
        CurrentFrameTime += Time.deltaTime;
        if (CurrentFrameTime > TimePerFrame)
        {
            CurrentFrameTime = 0f;
            animationSet.NextFrame(renderers);
        }
    }
}
public class UseTool : AnimateSprite
{

    float TotalFrameTime = 0f;
    private readonly PlayerController _controller;
    private readonly SpriteRenderer ToolRenderer;
    private TextureGroup ToolGroup;

    public UseTool(CharacterAnimationSet animationSet, SpriteRenderer[] renderers, PlayerController controller, SpriteRenderer toolRenderer) : base(animationSet, renderers)
    {
        _controller = controller;
        ToolRenderer = toolRenderer;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        ToolGroup = _controller.InventoryManager.HotBarHandler.SelectedHotbarAnimationSet.FirstOrDefault(a => a.Direction == animationSet.Direction);
        ToolGroup.nextSpriteNum = 0;
        TotalFrameTime = 0f;
    }

    public override void OnExit()
    {
        base.OnExit();
        _controller.CancelToolUse();
        ToolRenderer.sprite = null;
    }

    public override void Tick()
    {

        TotalFrameTime += Time.deltaTime;
        CurrentFrameTime += Time.deltaTime;

        if (CurrentFrameTime > TimePerFrame)
        {
            CurrentFrameTime = 0f;
            animationSet.NextFrame(renderers);
            if (ToolGroup != null)
            {
                ToolRenderer.sprite = ToolGroup.nextTexture();
            }

        }

        if (TotalFrameTime > TimePerFrame * animationSet.BaseLayer.Textures.Length)
        {
            _controller.UseTool();
        }

    }



}
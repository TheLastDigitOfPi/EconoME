using UnityEngine;

public class UseTool : AnimateSprite
{

    float TotalFrameTime = 0f;
    private readonly PlayerMovementController _controller;
    private readonly SpriteRenderer ToolRenderer;
    private TextureGroup ToolGroup;

    public UseTool(CharacterAnimationSet animationSet, SpriteRenderer[] renderers, PlayerMovementController controller, SpriteRenderer toolRenderer) : base(animationSet, renderers)
    {
        _controller = controller;
        ToolRenderer = toolRenderer;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        //ToolGroup = HotBarHandler.Instance.SelectedHotbarAnimationSet.FirstOrDefault(a => a.Direction == animationSet.Direction);
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
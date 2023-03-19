using UnityEngine;

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

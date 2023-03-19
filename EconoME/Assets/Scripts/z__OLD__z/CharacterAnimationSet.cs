using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Animation Set", menuName = "ScriptableObjects/Player/Animations/Character Animation Set")]
public class CharacterAnimationSet : ScriptableObject
{
    [SerializeField] TextureGroup _baseLayer;
    [SerializeField] TextureGroup _hair;
    [SerializeField] TextureGroup _shirt;
    [SerializeField] TextureGroup _pants;
    [SerializeField] TextureGroup _shoes;
    [SerializeField] float _secondsPerFrame;
    [SerializeField] Direction _direction;
    public Direction Direction { get { return _direction; } }

    public TextureGroup BaseLayer { get { return _baseLayer; } }
    public TextureGroup Hair { get { return _hair; } }
    public TextureGroup Shirt { get { return _shirt; } }
    public TextureGroup Pants { get { return _pants; } }
    public TextureGroup Shoes { get { return _shoes; } }
    public float SecondersPerFrame { get { return _secondsPerFrame; } }


    public void NextFrame(SpriteRenderer[] renderers)
    {
        if (renderers == null)
        {
            return;
        }
        if (_baseLayer == null)
        {
            Debug.Log(this.name + " has null layers");
            return;
        }

        renderers[0].sprite = _baseLayer.nextTexture();
        renderers[1].sprite = _hair.nextTexture();
        renderers[2].sprite = _shirt.nextTexture();
        renderers[3].sprite = _pants.nextTexture();
        renderers[4].sprite = _shoes.nextTexture();

    }

    internal void ResetAll()
    {
        _baseLayer.nextSpriteNum = 0;
        _hair.nextSpriteNum = 0;
        _shirt.nextSpriteNum = 0;
        _pants.nextSpriteNum = 0;
        _shoes.nextSpriteNum = 0;
    }
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}


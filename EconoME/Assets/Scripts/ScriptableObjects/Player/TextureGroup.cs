using UnityEngine;

[CreateAssetMenu(fileName = "New TextureGroup", menuName = "ScriptableObjects/Player/Animations/Texture Group")]
public class TextureGroup : ScriptableObject
{
    [SerializeField] Sprite[] _textures;
    [SerializeField] Direction _direction;

    public Direction Direction {get {return _direction;}}
    public Sprite[] Textures { get { return _textures; } }
    public int nextSpriteNum = 0;
    public Sprite nextTexture()
    {
        Sprite nextSprite = _textures[nextSpriteNum];
        nextSpriteNum++;
        if (nextSpriteNum >= _textures.Length)
        {
            nextSpriteNum = 0;
        }
        return nextSprite;
    }
}

using UnityEngine;

[CreateAssetMenu(fileName = "New Single Texture Group", menuName = "ScriptableObjects/Player/Animations/Single Texture Group")]
public class SingleTextureGroup : ScriptableObject
{
    [SerializeField] Sprite[] _textures;
    [SerializeField] float _timePerFrame;
    public float TimePerFrame { get { return _timePerFrame; } }
    public Sprite[] Textures { get { return _textures; } }
    public int CurrentSpriteNum { get; private set; } = 0;
    public Sprite NextTexture(bool forward = true)
    {
        return _textures[nextIndex(forward)];
    }
    public Sprite CurrentTexture
    {
        get
        {
            return _textures[CurrentSpriteNum];
        }
    }
    int nextIndex(bool forward)
    {
        CurrentSpriteNum += (forward ? 1 : -1);

        if (CurrentSpriteNum >= Textures.Length)
            CurrentSpriteNum = Textures.Length - 1;
        if (CurrentSpriteNum < 0)
            CurrentSpriteNum = 0;

        return CurrentSpriteNum;
    }
    public void resetToDefault()
    {
        CurrentSpriteNum = 0;
    }


}
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class BookOpenToggle : IChangeableState
{
    private readonly SingleTextureGroup animation;
    private readonly Image _image;
    private readonly BoolVariable bookOpen;
    private readonly GameObject LeftPage;
    private readonly GameObject RightPage;
    private readonly GameObject BookCoverText;
    private readonly CanvasGroup MainCanvas;
    private readonly GameObject Hotbar;
    float _currentTime;

    public bool isCompleted { get; private set; }
    bool finishedMoving = false;
    bool movingToScreen = false;
    bool movingBookxNeg = false;
    bool movingBookxPos = false;

    public BookOpenToggle(SingleTextureGroup anim, Image backgroundImage, BoolVariable bookOpen, GameObject LeftPage, GameObject RightPage, GameObject bookCoverText, CanvasGroup mainCanvas, GameObject hotbar)
    {
        animation = anim;
        _image = backgroundImage;
        this.bookOpen = bookOpen;
        this.RightPage = RightPage;
        this.LeftPage = LeftPage;
        BookCoverText = bookCoverText;
        MainCanvas = mainCanvas;
        Hotbar = hotbar;
    }

    public void OnEnter()
    {
        _image.sprite = animation.CurrentTexture;
        _currentTime = 0;
        isCompleted = false;
        if (bookOpen.Value)
        {
            Hotbar.SetActive(false);
            MainCanvas.ToggleCanvas();
            MainCanvas.transform.DOKill();
            MainCanvas.transform.DOLocalMoveY(50, 0.5f).SetEase(Ease.InOutSine).onComplete += () => { finishedMoving = true; movingBookxNeg = true; MainCanvas.transform.DOLocalMoveX(50, animation.TimePerFrame * animation.Textures.Length).onComplete += () => { movingBookxNeg = false; }; };
            movingToScreen = true;
            finishedMoving = false;
            return;
        }
    }

    public void OnExit()
    {
        MainCanvas.transform.DOKill();
    }
    public void Tick()
    {
        //If book is moving don't animate
        if (!finishedMoving)
        {
            //If book is moving and the value is changed, move back
            if (movingToScreen && !bookOpen.Value)
            {
                MainCanvas.transform.DOKill();
                MainCanvas.transform.DOLocalMoveY(-1100, 0.5f).SetEase(Ease.InOutSine).onComplete += () => { finishedMoving = true; isCompleted = true; MainCanvas.ToggleCanvas(); Hotbar.SetActive(true); };
                movingToScreen = false;
                finishedMoving = false;
                return;
            }
            if (!movingToScreen && bookOpen.Value)
            {
                MainCanvas.transform.DOKill();
                MainCanvas.transform.DOLocalMoveY(50, 0.5f).SetEase(Ease.InOutSine).onComplete += () => finishedMoving = true;
                movingToScreen = true;
                finishedMoving = false;
            }
            return;
        }
        //Animate book based off its time per frame
        _currentTime += Time.deltaTime;
        if (_currentTime > animation.TimePerFrame)
        {
            _currentTime = 0;
            _image.sprite = animation.NextTexture(bookOpen.Value);

            if (!bookOpen.Value && !movingBookxNeg)
            {
                movingBookxNeg = true;
                movingBookxPos = false;
                MainCanvas.transform.DOKill();
                MainCanvas.transform.DOLocalMoveX(-75, animation.TimePerFrame * animation.Textures.Length - 1).onComplete += () => { movingBookxNeg = false; };
            }
            if (bookOpen.Value && !movingBookxPos)
            {
                movingBookxPos = true;
                movingBookxNeg = false;
                MainCanvas.transform.DOKill();
                MainCanvas.transform.DOLocalMoveX(0, animation.TimePerFrame * animation.Textures.Length - 1).onComplete += () => { movingBookxPos = false; };
            }

            UpdatePageVisability();
            //If we are opening the book and it is fully open, we are done
            if (bookOpen.Value && _image.sprite == animation.Textures[animation.Textures.Length - 1])
            {
                isCompleted = true;
                return;
            }
            //If we are closing the book and it is fully closed, tween it away
            if (!bookOpen.Value && _image.sprite == animation.Textures[0])
            {
                MainCanvas.transform.DOKill();
                MainCanvas.transform.DOLocalMoveY(-1100, 0.5f).SetEase(Ease.InOutSine).onComplete += () => { finishedMoving = true; isCompleted = true; MainCanvas.ToggleCanvas(); Hotbar.SetActive(true); };
                movingToScreen = false;
                finishedMoving = false;
                return;
            }

            void UpdatePageVisability()
            {
                switch (animation.CurrentSpriteNum)
                {
                    case 0:
                        BookCoverText.SetActive(true);
                        return;
                    case 1:
                        BookCoverText.SetActive(false);
                        return;
                    case 2:
                        RightPage.SetActive(false);
                        return;
                    case 3:
                        RightPage.SetActive(true);
                        LeftPage.SetActive(false);
                        return;
                    case 4:
                        LeftPage.SetActive(true);
                        return;
                    default:
                        return;
                }
            }
        }
    }
}


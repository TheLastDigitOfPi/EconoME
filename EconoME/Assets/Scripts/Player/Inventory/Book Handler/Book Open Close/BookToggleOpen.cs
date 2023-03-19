using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BookToggleOpen : IChangeableState
{
    private readonly SingleTextureGroup animation;
    private readonly Image _image;
    private readonly BoolVariable _bookOpen;
    private readonly GameObject _leftPage;
    private readonly GameObject _rightPage;
    private readonly GameObject BookCoverText;
    private readonly CanvasGroup MainCanvas;
    private readonly GameObject Hotbar;
    float _currentTime;
    //If the book is current moving and not animating

    bool bookMoving { get { return isMovingX || isMovingY; } }

    bool isMovingX = false;
    bool isMovingXPositive = false;

    bool isMovingY = false;
    bool isMovingYPositive = false;

    public event Action OnBookOpen;
    public event Action OnBookClose;

    private readonly PlayerBookHandler _handler;

    const int ON_SCREEN_Y = 50;
    const int OFF_SCREEN_Y = -1100;

    const int BOOK_OPEN_X = 50;
    const int BOOK_Close_X = -75;

    public BookToggleOpen(PlayerBookHandler handler)
    {
        animation = handler.BookOpenCloseAnimation;
        _image = handler.BookBackground;
        _bookOpen = handler.InventoryOpen;
        _rightPage = handler.RightPage;
        _leftPage = handler.LeftPage;
        BookCoverText = handler.BookCoverText;
        MainCanvas = handler.MainInventoryCGroup;
        Hotbar = handler.Hotbar;
        _handler = handler;
    }


    public void OnEnter()
    {
        #region Thoughts
        /*

        If the book is currently moving, reverse the movement

            Is the book moving
            Which direction is the book moving

            4 Movements
            Move x Pos
            Move X Neg
            Move Y Pos
            Move Y Neg

            bool isMovingX
            bool isMovingY

            bool isMovingXPos
            bool isMovingYPos

            isMovingX OR isMovingY?

            if isMovingX -> isMovingxPos? MoveNeg (add Y Neg Movement): MovePos (done)

            if isMovingY -> isMovingYPos? MovYNeg : MoveYPos (add X Pos movement)

        otherwise

            Move YPos (add X Pos movement)

            MoveXNeg (add Y Neg Movement)


        */
        #endregion

        //If user toggled book while it is still moving, move it back
        if (bookMoving)
        {
            MainCanvas.transform.DOKill();

            if (isMovingX)
            {
                MoveBookX();
                return;
            }
            MoveBookY();
            return;
        }
        //Otherwise if book already moved to Pos X, move back. Otherwise move on Y
        if (isMovingXPositive)
        {
            MoveBookX();
            return;
        }
        MoveBookY();

        void MoveBookY()
        {
            isMovingY = true;
            //If moving towards close, reverse to open
            if (!isMovingYPositive)
            {
                isMovingYPositive = true;
                isMovingXPositive = true;

                Hotbar.SetActive(false);
                MainCanvas.SetCanvas(true);
                MainCanvas.transform.DOLocalMoveY(ON_SCREEN_Y, 0.5f).SetEase(Ease.InOutSine).onComplete += () =>
                {
                    isMovingX = true;
                    isMovingY = false;
                    MainCanvas.transform.DOLocalMoveX(BOOK_OPEN_X, animation.TimePerFrame * animation.Textures.Length).onComplete += () => isMovingX = false;
                };
                return;
            }
            //Otherwise reverse to close
            isMovingYPositive = false;
            isMovingXPositive = false;
            MainCanvas.transform.DOLocalMoveY(OFF_SCREEN_Y, 0.5f).SetEase(Ease.InOutSine).onComplete += OffScreen;
        }
        void MoveBookX()
        {
            isMovingX = true;
            //If moving towards open, reverse to close
            if (isMovingXPositive)
            {
                //Move X Neg
                isMovingXPositive = false;
                isMovingYPositive = false;
                MainCanvas.transform.DOLocalMoveX(BOOK_Close_X, animation.TimePerFrame * animation.Textures.Length).onComplete += () =>
                {

                    isMovingY = true;
                    isMovingX = false;
                    MainCanvas.transform.DOLocalMoveY(OFF_SCREEN_Y, 0.5f).SetEase(Ease.InOutSine).onComplete += OffScreen;
                };

                return;
            }
            //Otherwise reverse to open
            isMovingXPositive = true;
            isMovingYPositive = true;

            MainCanvas.transform.DOLocalMoveX(BOOK_OPEN_X, animation.TimePerFrame * animation.Textures.Length).onComplete += () =>
            {
                isMovingX = false;
            };

        }
        void OffScreen()
        {
            MainCanvas.ToggleCanvas();
            Hotbar.SetActive(true);
            isMovingY = false;
            _handler.CurrentStateFinished(this);
        }
    }

    public void OnExit()
    {
    }
    public void Tick()
    {
        //If book is not moving, don't animate
        if (!isMovingX && (animation.CurrentSpriteNum == 0 || animation.CurrentSpriteNum == animation.Textures.Length - 1))
            return;

        //Animate book based off its time per frame

        _currentTime += Time.deltaTime;
        if (_currentTime > animation.TimePerFrame)
        {
            _currentTime = 0;
            _image.sprite = animation.NextTexture(_bookOpen.Value);

            UpdatePageVisability();
            //If we are opening the book and it is fully open, we are done
            if (_bookOpen.Value && _image.sprite == animation.Textures[animation.Textures.Length - 1])
            {
                _handler.CurrentStateFinished(this);
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
                        _rightPage.SetActive(false);
                        return;
                    case 3:
                        _rightPage.SetActive(_handler.HasActiveRightPage);
                        _leftPage.SetActive(false);
                        OnBookClose?.Invoke();
                        return;
                    case 4:
                        _leftPage.SetActive(_handler.HasActiveLeftPage);
                        OnBookOpen?.Invoke();
                        return;
                    default:
                        return;
                }
            }

        }
    }
}


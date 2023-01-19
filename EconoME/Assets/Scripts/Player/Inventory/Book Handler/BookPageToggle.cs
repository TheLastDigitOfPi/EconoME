using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BookPageToggle : IChangeableState
{
    //Toggles the book pages as they flip between another
    public bool isCompleted { get; private set; }

    private readonly Image BookBackground;
    private readonly GameObject LeftPageMask;
    private readonly GameObject RightPageMask;
    private readonly SingleTextureGroup FlipRightAnimation;
    private readonly SingleTextureGroup FlipLeftAnimation;
    private readonly Transform LeftPageHolder;
    private readonly Transform RightPageHolder;
    private readonly TextMeshProUGUI LeftPageTitle;
    private readonly TextMeshProUGUI RightPageTitle;
    private readonly Image LeftPageIcon;
    private readonly Image RightPageIcon;

    private BookPage ActiveLeftPage;
    private BookPage ActiveRightPage;
    private BookPage NewLeftPage;
    private BookPage NewRightPage;


    private int _totalPageFlips = 1;
    private int _currentPageFlips = 0;
    private float _currentTime;
    private bool FlipRight = false;
    private float _flipSpeedMultiplier = 1;

    public BookPageToggle(Image bookBackground, GameObject leftPageMask, GameObject rightPageMask, SingleTextureGroup flipRightAnimation, SingleTextureGroup flipLeftAnimation, BookPage originalActiveLeftPage, BookPage originalActiveRightPage, TextMeshProUGUI leftPageTitle, TextMeshProUGUI rightPageTitle, Image leftPageIcon, Image rightPageIcon)
    {
        BookBackground = bookBackground;
        LeftPageMask = leftPageMask;
        RightPageMask = rightPageMask;
        FlipRightAnimation = flipRightAnimation;
        FlipLeftAnimation = flipLeftAnimation;
        ActiveLeftPage = originalActiveLeftPage;
        ActiveRightPage = originalActiveRightPage;
        LeftPageHolder = leftPageMask.transform.GetChild(0);
        RightPageHolder = rightPageMask.transform.GetChild(0);
        LeftPageTitle = leftPageTitle;
        RightPageTitle = rightPageTitle;
        LeftPageIcon = leftPageIcon;
        RightPageIcon = rightPageIcon;
    }

    private SingleTextureGroup CurrentAnimation { get { return FlipRight ? FlipRightAnimation : FlipLeftAnimation; } }

    public void setFlip(bool flipRight, BookPage LeftPage, BookPage RightPage, int pageFlips = 1, float flipSpeedMultiplier = 1)
    {
        FlipRight = flipRight;
        NewLeftPage = LeftPage;
        NewRightPage = RightPage;

        if (NewLeftPage == null)
            NewLeftPage = ActiveLeftPage;
        if (NewRightPage == null)
            NewRightPage = ActiveRightPage;

        _totalPageFlips = pageFlips;
        _currentPageFlips = 0;
        _flipSpeedMultiplier = flipSpeedMultiplier;

        if (pageFlips > 1)
        {
            RightPageMask.SetActive(false);
            LeftPageMask.SetActive(false);
        }

    }

    public void OnEnter()
    {
        isCompleted = false;
        FlipRightAnimation.resetToDefault();
        FlipLeftAnimation.resetToDefault();
        _currentPageFlips = 0;

        if (CurrentAnimation == null) { return; }
        BookBackground.sprite = CurrentAnimation.CurrentTexture;
    }

    public void OnExit()
    {

    }

    public void Tick()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime > CurrentAnimation.TimePerFrame / _flipSpeedMultiplier)
        {
            _currentTime = 0;

            BookBackground.sprite = CurrentAnimation.NextTexture();
            if (_currentPageFlips + 1 >= _totalPageFlips)
            {
                UpdatePageVisability();
            }
            //If we are  the book and it is fully open, we are done
            if (BookBackground.sprite == CurrentAnimation.Textures.Last())
            {
                _currentPageFlips++;
                if (_currentPageFlips >= _totalPageFlips)
                {
                    isCompleted = true;
                    CurrentAnimation.resetToDefault();
                    return;
                }
                CurrentAnimation.resetToDefault();

            }

            void UpdatePageVisability()
            {
                if (FlipRight)
                {
                    switch (CurrentAnimation.CurrentSpriteNum)
                    {
                        case 1:
                            //set Left pages invisible
                            LeftPageMask.SetActive(false);
                            //Swap pages while invisible
                            ActiveLeftPage.gameObject.SetActive(false);
                            NewLeftPage.SetPage(LeftPageHolder, LeftPageIcon, LeftPageTitle);
                            ActiveLeftPage = NewLeftPage;
                            NewLeftPage = null;
                            return;
                        case 2:
                            LeftPageMask.SetActive(true);
                            (RightPageMask.transform as RectTransform).sizeDelta = new Vector2(483, 0);
                            return;
                        case 3:
                            //Set Right pages invisible
                            RightPageMask.SetActive(false);
                            //Swap pages while invisible
                            ActiveRightPage.gameObject.SetActive(false);
                            NewRightPage.SetPage(RightPageHolder, RightPageIcon, RightPageTitle);
                            ActiveRightPage = NewRightPage;
                            NewRightPage = null;
                            return;
                        case 5:
                            RightPageMask.SetActive(true);
                            (RightPageMask.transform as RectTransform).sizeDelta = new Vector2(540, 0);
                            return;
                        default:
                            return;
                    }
                }

                switch (CurrentAnimation.CurrentSpriteNum)
                {
                    case 1:
                        //Set Right pages invisible
                        RightPageMask.SetActive(false);
                        //Swap pages while invisible
                        ActiveRightPage.gameObject.SetActive(false);
                        NewRightPage.SetPage(RightPageHolder, RightPageIcon, RightPageTitle);
                        ActiveRightPage = NewRightPage;
                        NewRightPage = null;
                        return;
                    case 2:
                        RightPageMask.SetActive(true);
                        (LeftPageMask.transform as RectTransform).sizeDelta = new Vector2(494, 0);
                        return;
                    case 3:
                        //Set Left pages invisible
                        LeftPageMask.SetActive(false);
                        //Swap pages while invisible
                        ActiveLeftPage.gameObject.SetActive(false);
                        NewLeftPage.SetPage(LeftPageHolder, LeftPageIcon, LeftPageTitle);
                        ActiveLeftPage = NewLeftPage;
                        NewLeftPage = null;
                        return;
                    case 5:
                        LeftPageMask.SetActive(true);
                        (LeftPageMask.transform as RectTransform).sizeDelta = new Vector2(540, 0);
                        return;
                    default:
                        return;
                }

            }
        }
    }
}


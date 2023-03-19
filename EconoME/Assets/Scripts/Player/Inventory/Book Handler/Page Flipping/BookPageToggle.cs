using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class BookPageToggle : IChangeableState
{
    //Toggles the book pages as they flip between another

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

    public BookPage ActiveLeftPage { get; set; }
    public BookPage ActiveRightPage { get; set; }
    private BookPage NewLeftPage;
    private BookPage NewRightPage;


    private int _totalPageFlips = 1;
    private int _currentPageFlips = 0;
    private float _currentTime;
    private bool FlipRight = false;
    private float _flipSpeedMultiplier = 1;
    PlayerBookHandler _handler;

    public BookPageToggle(PlayerBookHandler handler)
    {
        BookBackground = handler.BookBackground;
        LeftPageMask = handler.LeftPage;
        RightPageMask = handler.RightPage;
        FlipRightAnimation = handler.BookFlipRightAnimation;
        FlipLeftAnimation = handler.BookFlipLeftAnimation;
        LeftPageHolder = LeftPageMask.transform.GetChild(0);
        RightPageHolder = RightPageMask.transform.GetChild(0);
        LeftPageTitle = handler.LeftPageTitle;
        RightPageTitle = handler.RightPageTitle;
        LeftPageIcon = handler.LeftPageImage;
        RightPageIcon = handler.RightPageImage;
        _handler = handler;
    }

    private SingleTextureGroup CurrentAnimation { get { return FlipRight ? FlipRightAnimation : FlipLeftAnimation; } }

    public void setFlip(bool flipRight, BookPage LeftPage, BookPage RightPage, int pageFlips = 1, float flipSpeedMultiplier = 1)
    {
        FlipRight = flipRight;
        NewLeftPage = LeftPage;
        NewRightPage = RightPage;

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

        FlipRightAnimation.resetToDefault();
        FlipLeftAnimation.resetToDefault();
        _currentPageFlips = 0;

        if (CurrentAnimation == null) { return; }
        BookBackground.sprite = CurrentAnimation.CurrentTexture;
    }

    public void OnExit()
    {
        CurrentAnimation.resetToDefault();
    }

    public void Tick()
    {
        //Update page visibility and book animation
        _currentTime += Time.deltaTime;
        if (_currentTime < CurrentAnimation.TimePerFrame / _flipSpeedMultiplier)
            return;

        _currentTime = 0;
        BookBackground.sprite = CurrentAnimation.NextTexture();

        //Only update the page visability if we are on the final flip
        if (_currentPageFlips == _totalPageFlips - 1)
            UpdatePageVisability();

        //If at final animation we are done with a flip
        if (BookBackground.sprite == CurrentAnimation.Textures.Last())
        {
            _currentPageFlips++;
            if (_currentPageFlips >= _totalPageFlips)
            {
                _handler.CurrentStateFinished(this);
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
                        ActiveLeftPage?.gameObject.SetActive(false);
                        NewLeftPage?.SetPage(LeftPageHolder, LeftPageIcon, LeftPageTitle);
                        ActiveLeftPage = NewLeftPage;
                        NewLeftPage = null;
                        return;
                    case 2:
                        //Left Page Visible Now
                        LeftPageMask.SetActive(ActiveLeftPage != null);
                        (RightPageMask.transform as RectTransform).sizeDelta = new Vector2(483, 0);
                        return;
                    case 3:
                        //Set Right pages invisible
                        RightPageMask.SetActive(false);
                        //Swap pages while invisible
                        ActiveRightPage?.gameObject.SetActive(false);
                        NewRightPage?.SetPage(RightPageHolder, RightPageIcon, RightPageTitle);
                        ActiveRightPage = NewRightPage;
                        NewRightPage = null;
                        return;
                    case 5:
                        RightPageMask.SetActive(ActiveRightPage != null);
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
                    ActiveRightPage?.gameObject.SetActive(false);
                    NewRightPage?.SetPage(RightPageHolder, RightPageIcon, RightPageTitle);
                    ActiveRightPage = NewRightPage;
                    NewRightPage = null;
                    return;
                case 2:
                    RightPageMask.SetActive(ActiveRightPage != null);
                    (LeftPageMask.transform as RectTransform).sizeDelta = new Vector2(494, 0);
                    return;
                case 3:
                    //Set Left pages invisible
                    LeftPageMask.SetActive(false);
                    //Swap pages while invisible
                    ActiveLeftPage?.gameObject.SetActive(false);
                    NewLeftPage?.SetPage(LeftPageHolder, LeftPageIcon, LeftPageTitle);
                    ActiveLeftPage = NewLeftPage;
                    NewLeftPage = null;
                    return;
                case 5:
                    LeftPageMask.SetActive(ActiveLeftPage != null);
                    (LeftPageMask.transform as RectTransform).sizeDelta = new Vector2(540, 0);
                    return;
                default:
                    return;
            }

        }
    }

}


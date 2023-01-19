using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections.Generic;

//Handles common function of book interactions using custom state machine. Ex: Opens/Closes Book UI, Flips Pages
public class PlayerBookHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI PlayerMoneyText;
    [SerializeField] CanvasGroup MainInventoryCGroup;
    [SerializeField] Image BookBackground;
    [SerializeField] GameObject BookCoverText;
    [SerializeField] GameObject Hotbar;

    [Space(10)]
    [Header("Global Variables")]
    [SerializeField] IntVariable PlayerCurrency;
    [SerializeField] BoolVariable InventoryOpen;

    [Space(10)]
    [Header("Page Sides")]
    [SerializeField] GameObject LeftPage;
    [SerializeField] Image LeftPageImage;
    [SerializeField] TextMeshProUGUI LeftPageTitle;
    [SerializeField] GameObject RightPage;
    [SerializeField] Image RightPageImage;
    [SerializeField] TextMeshProUGUI RightPageTitle;

    [Space(10)]
    [Header("Original Pages")]
    [SerializeField] BookPage OriginalLeftPage;
    [SerializeField] BookPage OriginalRightPage;

    [Space(10)]
    [Header("Pages")]
    [SerializeField] BookPage Backpack;
    [SerializeField] BookPage ResourceBag;
    [SerializeField] BookPage Status;
    [SerializeField] BookPage Info;
    [SerializeField] BookPage SettingsSelector;
    [SerializeField] BookPage Graphics;
    [SerializeField] BookPage Audio;
    [SerializeField] BookPage Controls;
    [SerializeField] BookPage General;

    //Texture Groups
    [SerializeField] SingleTextureGroup _bookOpenCloseAnimation;
    [SerializeField] SingleTextureGroup bookFlipLeftAnimation;
    [SerializeField] SingleTextureGroup bookFlipRightAnimation;

    //States to toggle
    BookToggleTest _openBookToggle;
    BookPageToggle bookPageToggle;
    IChangeableState CurrentState;

    //Can't toggle book while pages are flipping or possible new states such as animations
    bool CanToggleBookOpen { get { return CurrentState == null || CurrentState == _openBookToggle; } }
    private void Awake()
    {
        _openBookToggle = new(_bookOpenCloseAnimation, BookBackground, InventoryOpen, LeftPage, RightPage, BookCoverText, MainInventoryCGroup, Hotbar, this);
        bookPageToggle = new(BookBackground, LeftPage, RightPage, bookFlipRightAnimation, bookFlipLeftAnimation, OriginalLeftPage, OriginalRightPage, LeftPageTitle, RightPageTitle, LeftPageImage, RightPageImage);
        _bookOpenCloseAnimation.resetToDefault();
    }
    private void Start()
    {
        PlayerCurrency.onValueChange += UpdateGold;
        InventoryOpen.onValueChange += ToggleBook;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (CanToggleBookOpen)
                InventoryOpen.Value = !InventoryOpen.Value;
        }
        if (CurrentState == null)
            return;
        CurrentState.Tick();

    }
    private void OnDisable()
    {
        PlayerCurrency.onValueChange -= UpdateGold;
        InventoryOpen.onValueChange -= ToggleBook;
        InventoryOpen.Value = false;
    }
    public void UpdateGold()
    {
        if (PlayerMoneyText != null && PlayerMoneyText.isActiveAndEnabled)
        {
            PlayerMoneyText.text = PlayerCurrency.Value.ToString();
        }
    }
    private void ToggleBook()
    {
        SetCurrentState(_openBookToggle);
    }
    public void FlipToResourceInventory()
    {
        Flip(false, Info, ResourceBag);
    }
    public void FlipToBackpack()
    {
        Flip(true, Status, Backpack);
    }
    public void Flip(bool flipRight, BookPage LeftPage, BookPage RightPage)
    {
        if (!InventoryOpen.Value)
            return;
        if (SetCurrentState(bookPageToggle))
            bookPageToggle.setFlip(flipRight, LeftPage, RightPage);
    }
    private bool SetCurrentState(IChangeableState newState)
    {
        CurrentState?.OnExit();
        CurrentState = newState;
        CurrentState.OnEnter();
        return true;
    }

    public void CurrentStateFinished(IChangeableState finishedState)
    {
        if (finishedState == CurrentState)
        {
            CurrentState.OnExit();
            CurrentState = null;
        }

    }



}


using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

//Handles common function of book interactions using custom state machine. Ex: Opens/Closes Book UI, Flips Pages
public class PlayerBookHandler : MonoBehaviour
{
    #region Fields
    [field: Header("General")]
    [field: SerializeField] public CanvasGroup MainInventoryCGroup { get; private set; }
    [field: SerializeField] public Image BookBackground { get; private set; }
    [field: SerializeField] public GameObject BookCoverText { get; private set; }
    [field: SerializeField] public GameObject Hotbar { get; private set; }
    [field: SerializeField] public BookTabManager TabManager { get; private set; }

    [field: Space(10)]
    [field: Header("Global Variables")]
    [field: SerializeField] public IntVariable PlayerCurrency { get; private set; }
    [field: SerializeField] public BoolVariable InventoryOpen { get; private set; }

    [field: Space(10)]
    [field: Header("Page Sides")]
    [field: SerializeField] public GameObject LeftPage { get; private set; }
    [field: SerializeField] public Image LeftPageImage { get; private set; }
    [field: SerializeField] public TextMeshProUGUI LeftPageTitle { get; private set; }
    [field: SerializeField] public GameObject RightPage { get; private set; }
    [field: SerializeField] public Image RightPageImage { get; private set; }
    [field: SerializeField] public TextMeshProUGUI RightPageTitle { get; private set; }

    public BookPage CurrentLeftPage { get { return _bookPageToggle.ActiveLeftPage; } }
    public BookPage CurrentRightPage { get { return _bookPageToggle.ActiveRightPage; } }

    [field: Space(10)]
    [field: Header("Pages")]
    [field: SerializeField] public Transform PagesParent { get; private set; }

    [field: Space(10)]
    [field: Header("Texture Groups")]
    [field: SerializeField] public SingleTextureGroup BookOpenCloseAnimation { get; private set; }
    [field: SerializeField] public SingleTextureGroup BookFlipLeftAnimation { get; private set; }
    [field: SerializeField] public SingleTextureGroup BookFlipRightAnimation { get; private set; }


    public bool HasActiveLeftPage { get { return _bookPageToggle.ActiveLeftPage != null; } }
    public bool HasActiveRightPage { get { return _bookPageToggle.ActiveRightPage != null; } }

    public static event Action OnBookOpen { add { Instance._openBookToggle.OnBookOpen += value; } remove { Instance._openBookToggle.OnBookOpen -= value; } }
    public static event Action OnBookClose { add { Instance._openBookToggle.OnBookClose += value; } remove { Instance._openBookToggle.OnBookClose -= value; } }

    //States to toggle
    BookToggleOpen _openBookToggle;
    BookPageToggle _bookPageToggle;
    IChangeableState CurrentState;


    public static PlayerBookHandler Instance;

    //Can't toggle book while pages are flipping or possible new states such as animations
    bool CanToggleBook { get { return (CurrentState == null || CurrentState == _openBookToggle) && !_bookLocked && !ChatBoxManager.Instance.ChatBoxActive;} }

    bool _bookLocked = false;
    private InputAction toggleInventory;
    #endregion


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than 1 instance of player book handler found, killing latest");
            Destroy(this);
            return;
        }
        Instance = this;
        _openBookToggle = new(this);
        _bookPageToggle = new(this);
        BookOpenCloseAnimation.resetToDefault();
        toggleInventory = CustomInputManager.Instance.Tab;
        BookPage[] pages = PagesParent.GetComponentsInChildren<BookPage>(true);
        foreach (var page in pages)
        {
            page.Initialize(this);
        }
    }



    internal void CloseBook()
    {
        if (InventoryOpen.Value)
            InventoryOpen.Value = false;
        _bookLocked = false;
    }

    internal void UnlockPage(UnlockableInventory pageToUnlock)
    {
        bool unsubscribe = false;
        _bookLocked = true;
        if (!InventoryOpen.Value)
        {
            InventoryOpen.Value = true;
            _openBookToggle.OnBookOpen += StartFlips;
            unsubscribe = true;
            return;
        }
        StartFlips();
        void StartFlips()
        {
            pageToUnlock.UnlockInventory();
            Flip(pageToUnlock, flips: 5, flipSpeed: 3f);
            if (unsubscribe)
                _openBookToggle.OnBookOpen -= StartFlips;
        }

    }



    private void Start()
    {
        _openBookToggle.OnBookOpen += TabManager.EnableTabs;
        _openBookToggle.OnBookClose += TabManager.DisableTabs;
    }

    private void OnDestroy()
    {
        InventoryOpen.onValueChange -= ToggleBook;
    }

    public void InitializeStartingPages(BookPage leftPage, BookPage rightPage)
    {
        _bookPageToggle.ActiveLeftPage = leftPage;
        _bookPageToggle.ActiveRightPage = rightPage;

        leftPage?.SetPage(LeftPage.transform.GetChild(0), LeftPageImage, LeftPageTitle);
        rightPage?.SetPage(RightPage.transform.GetChild(0), RightPageImage, RightPageTitle);
    }

    private void OnEnable()
    {
        CustomInputManager.Instance.Tab.started += ToggleInventory_started;
        InventoryOpen.onValueChange += ToggleBook;
    }

    private void ToggleInventory_started(InputAction.CallbackContext obj)
    {
        if (CanToggleBook)
            InventoryOpen.Value = !InventoryOpen.Value;
    }

    [ContextMenu("Set For Edit")]
    public void SetForEdit()
    {
        MainInventoryCGroup.SetCanvas(true);
        BookBackground.sprite = BookOpenCloseAnimation.Textures.Last();
        BookCoverText.SetActive(false);
        LeftPage.SetActive(true);
        RightPage.SetActive(true);
    }
    [ContextMenu("Set For Prod")]
    public void SetForProd()
    {
        MainInventoryCGroup.SetCanvas(false);
        BookCoverText.SetActive(true);
        LeftPage.SetActive(false);
        RightPage.SetActive(false);
        BookBackground.sprite = BookOpenCloseAnimation.Textures.First();
    }

    private void Update()
    {
        if (CurrentState == null)
            return;
        CurrentState.Tick();

    }
    private void OnDisable()
    {
        toggleInventory.started -= ToggleInventory_started;
        InventoryOpen.onValueChange -= ToggleBook;
        InventoryOpen.Value = false;
    }

    private void ToggleBook()
    {
        SetCurrentState(_openBookToggle);
    }

    public void Flip(BookPage LeftPage, BookPage RightPage, int pageFlips = 1, float flipSpeedMultiplier = 1f, bool flipRight = false)
    {
        if (!InventoryOpen.Value || CurrentState == _bookPageToggle)
            return;
        if (SetCurrentState(_bookPageToggle))
            _bookPageToggle.setFlip(flipRight, LeftPage, RightPage, pageFlips: pageFlips, flipSpeedMultiplier: flipSpeedMultiplier);
    }

    internal void OnRemoveTab(BookPage connectedPage, BookPage replacedPage = null)
    {
        //If page removed was active on the screen, replace with the next available page
        if (connectedPage == _bookPageToggle.ActiveLeftPage)
        {
            Flip(replacedPage, _bookPageToggle.ActiveRightPage);
            return;
        }

        if (connectedPage == _bookPageToggle.ActiveRightPage)
        {
            Flip(_bookPageToggle.ActiveLeftPage, replacedPage);
            return;
        }
    }

    internal void OnAddTab(UnlockableInventory inventory)
    {
        if (inventory.TabSide == TabSide.Right && _bookPageToggle.ActiveRightPage == null)
        {
            _bookPageToggle.ActiveRightPage = inventory.ConnectedPage;
            Flip(_bookPageToggle.ActiveLeftPage, _bookPageToggle.ActiveRightPage, flipRight: inventory.TabSide != TabSide.Right);
            return;
        }
        if (inventory.TabSide == TabSide.Left && _bookPageToggle.ActiveLeftPage == null)
        {
            _bookPageToggle.ActiveLeftPage = inventory.ConnectedPage;
            Flip(_bookPageToggle.ActiveLeftPage, _bookPageToggle.ActiveRightPage, flipRight: inventory.TabSide != TabSide.Right);
        }

    }

    public void Flip(UnlockableInventory inventoryToOpen, int flips = 1, float flipSpeed = 1f)
    {
        if (FindInventory(inventoryToOpen, out BookPage foundPage))
        {
            if (inventoryToOpen.TabSide == TabSide.Left)
                Flip(foundPage, _bookPageToggle.ActiveRightPage, flipRight: inventoryToOpen.TabSide != TabSide.Right, pageFlips: flips, flipSpeedMultiplier: flipSpeed);
            else
                Flip(_bookPageToggle.ActiveLeftPage, foundPage, flipRight: inventoryToOpen.TabSide != TabSide.Right, pageFlips: flips, flipSpeedMultiplier: flipSpeed);
            return;
        }
        Debug.LogWarning("Unable find page for inventory: " + inventoryToOpen.name);
    }

    bool FindInventory(UnlockableInventory inventoryToFind, out BookPage foundPage)
    {
        foundPage = null;
        if (inventoryToFind.ConnectedPage == null)
            return false;

        foundPage = inventoryToFind.ConnectedPage;
        return true;
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


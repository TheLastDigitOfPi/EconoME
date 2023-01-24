using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookTabManager : MonoBehaviour
{
    [field: SerializeField] public GridLayoutGroup LeftTabsParent { get; private set; }
    [field: SerializeField] public GridLayoutGroup RightTabsParent { get; private set; }
    [field: SerializeField] public BookTab TabPrefab { get; private set; }
    [field: SerializeField] public HeldBookTab HeldTabPrefab { get; private set; }
    [field: SerializeField] public float MoveDistance { get; private set; }
    [field: SerializeField] public PlayerBookHandler PlayerBookHandler { get; private set; }
    [SerializeField] AllInventories EnabledInventories;

    private void Start()
    {
        LeftTabsParent.gameObject.SetActive(true);
        RightTabsParent.gameObject.SetActive(true);

        BookPage initialLeftPage = null;
        BookPage initialRightPage = null;
        //Create tabs for each currently available inventory
        foreach (var inventory in EnabledInventories.Inventories)
        {
            if (inventory.IsUnlocked)
            {
                AddTab(inventory);
                //Check if we have a starting page, if not set this to the starting page
                if (initialLeftPage == null && inventory.TabSide == TabSide.Left)
                    initialLeftPage = inventory.ConnectedPage;
                if (initialRightPage == null && inventory.TabSide == TabSide.Right)
                    initialRightPage = inventory.ConnectedPage;
                continue;
            }
            inventory.OnUnlock += AddTab;
        }
        PlayerBookHandler.InitializeStartingPages(initialLeftPage, initialRightPage);
        SortTabs(RightTabsParent);
        SortTabs(LeftTabsParent, disableParents: true);
    }

    public void AddTab(UnlockableInventory inventory)
    {
        GridLayoutGroup tabSide = inventory.TabSide == TabSide.Left ? LeftTabsParent : RightTabsParent;
        var NewTab = Instantiate(TabPrefab, tabSide.transform);
        if(inventory.TabSide == TabSide.Left)
            NewTab.transform.SetAsFirstSibling();
        NewTab.Initialize(inventory, this);
        NewTab.OnClick += FlipToPage;
        SortTabs(tabSide, NewTab);
        PlayerBookHandler.OnAddTab(inventory);
    }

    public void EnableTabs()
    {
        LeftTabsParent.gameObject.SetActive(true);
        RightTabsParent.gameObject.SetActive(true);
    }

    public void DisableTabs()
    {
        LeftTabsParent.gameObject.SetActive(false);
        RightTabsParent.gameObject.SetActive(false);
    }

    private void FlipToPage(UnlockableInventory inventory)
    {
        if (!IsTabOpen(inventory))
            PlayerBookHandler.Flip(inventory);

        bool IsTabOpen(UnlockableInventory tabToCheck)
        {
            if (tabToCheck.TabSide == TabSide.Right)
                return PlayerBookHandler.CurrentRightPage == tabToCheck.ConnectedPage;

            return PlayerBookHandler.CurrentLeftPage == tabToCheck.ConnectedPage;
        }
    }



    void SortTabs(GridLayoutGroup tabSide, BookTab tab = null, bool disableParents = false)
    {
        StartCoroutine(sortTabs());
        IEnumerator sortTabs()
        {
            tabSide.enabled = true;
            yield return null;
            tabSide.enabled = false;
            if (tab != null)
                tab.InitialX = (tab.transform as RectTransform).localPosition.x;
            if (disableParents)
            {
                LeftTabsParent.gameObject.SetActive(false);
                RightTabsParent.gameObject.SetActive(false);
            }
        }
    }


    public void RemoveTab(BookTab removedTab)
    {
        UnlockableInventory inventory = removedTab.Inventory;
        Destroy(removedTab.gameObject);
        SortTabs(inventory.TabSide == TabSide.Left ? LeftTabsParent : RightTabsParent);
        BookPage replacementPage = null;
        BookTab[] replacementOptions = null;
        if (inventory.TabSide == TabSide.Right)
            replacementOptions = RightTabsParent.transform.GetComponentsInChildren<BookTab>();
        if (inventory.TabSide == TabSide.Left)
            replacementOptions = LeftTabsParent.transform.GetComponentsInChildren<BookTab>();

        //Find first tab that isn't the one we just removed
        foreach (var tab in replacementOptions)
        {
            if (tab.Inventory.ConnectedPage != inventory.ConnectedPage)
            {
                replacementPage = tab.Inventory.ConnectedPage;
                break;
            }
        }

        PlayerBookHandler.OnRemoveTab(removedTab.Inventory.ConnectedPage, replacementPage);
    }

    public void ShowTabGroup()
    {

    }
}

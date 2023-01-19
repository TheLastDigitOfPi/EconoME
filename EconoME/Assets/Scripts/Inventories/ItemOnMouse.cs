using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class ItemOnMouse : MonoBehaviour
{
    //MoveMousItem script: Allows player to grab item from inventory 
    //and drag around/place in other inventory slots

    CanvasGroup movingItemCanvasGroup;
    TextMeshProUGUI TMP;
    Image itemImage;

    public Item item;
    public GameObject FloorItemPrefab;

    public bool HoldingItem{get; private set;}

    PlayerInventoryManager inventoryManager; 
    private void Awake()
    {
        inventoryManager = GetComponentInParent<PlayerInventoryManager>();
        
        movingItemCanvasGroup = GetComponent<CanvasGroup>();
        TMP = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        itemImage = gameObject.GetComponentInChildren<Image>();
    }
    private void Start()
    {
    }

    GameObject foundObject;
    void Update()
    {
        //Drop item on ground if didn't click on UI
        if (HoldingItem)
        {
            transform.position = Mouse.current.position.ReadValue() + new Vector2(15, 15);
            if (Input.GetMouseButtonDown(0))
            {
                if (!ClickedOnUI())
                {
                    DropItem();
                    return;
                }
                if (foundObject.name == "Trash")
                {
                    Trash();
                }

            }
        }

    }
    bool ClickedOnUI()
    {
        var results = new List<RaycastResult>();
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(eventData, results);
        if (results.Count > 0) { foundObject = results[0].gameObject; } else { foundObject = null; }
        return results.Count > 0;
    }

    public void ClickedOnSlot(InventorySlotHandler slotClicked)
    {
        #region Check for Shift Click to Inventory
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (slotClicked.slotData.isEmpty) { return; }

            var InventoryGroup = GetInventoryGroup(slotClicked);
            if (InventoryGroup != null)
            {
                //Add it to that group
                if (InventoryManager.AddToInventoryGroup(InventoryGroup, slotClicked.slotData.item))
                {
                    slotClicked.ClearSlot();
                    return;
                }
                slotClicked.UpdateSlot();
                return;
            }

        }
        #endregion
        //Otherwise attempt to grab/replace/drop item
        if (HoldingItem)
        {
            DropItem(slotClicked);
            return;
        }
        GrabItem(slotClicked);
    }

    private bool GrabItem(InventorySlotHandler inventorySlot)
    {
        //Check if valid slot
        if (!inventorySlot.slotData.CanGrabSlot())
        {
            return false;
        }
        inventorySlot.GrabItem(out item);


        //Set Image
        itemImage.sprite = item.Icon;
        itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 1);
        //Set Text
        TMP.text = item.Stacksize > 1 ? item.Stacksize.ToString() : "";

        movingItemCanvasGroup.alpha = 1;
        HoldingItem = true;
        transform.position = Mouse.current.position.ReadValue() + new Vector2(15, 15);
        return true;

    }

    private void SwapItems(InventorySlotHandler SlotToSwap)
    {
        var tempItemHolder = SlotToSwap.slotData.item;

        if (!SlotToSwap.SetSlot(item)) { return; }
        item = tempItemHolder;
        itemImage.sprite = item.Icon;
        TMP.text = item.Stacksize.ToString();

    }

    //Drop Item Into World
    bool DropItem()
    {
        //Item dropped into world
        if (!HoldingItem) { return false; }

        GameObject newItem = Instantiate(FloorItemPrefab);
        WorldItemHandler worldItem = newItem.GetComponent<WorldItemHandler>();
        worldItem.data.item = item;

        worldItem.updateImage();
        worldItem.updateText();

        ClearItem();

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        newItem.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        newItem.transform.position = new Vector3(newItem.transform.position.x, newItem.transform.position.y, 0);

        HoldingItem = false;
        return false;

    }

    internal void Trash()
    {
        if (HoldingItem) { ClearItem(); }
    }

    //Drop Item To Inventory Slot - Return true if all items were successfuly dropped
    private bool DropItem(InventorySlotHandler IS)
    {
        if (item == null || item.Stacksize <= 0) { return false; }

        if (IS.SetSlot(item)) { ClearItem(); return true; }

        //Item was not all put in slot, update UI
        TMP.text = item.Stacksize.ToString();
        return false;
    }

    private void ClearItem()
    {
        HoldingItem = false;
        TMP.text = "";
        item = null;
        itemImage.sprite = null;
        itemImage.color = new Color(itemImage.color.r, itemImage.color.g, itemImage.color.b, 0);
    }

    [SerializeField] BoolVariable ChestOpen;
    [SerializeField] BoolVariable TileMakerOpen;
    [SerializeField] BoolVariable BankOpen;
    [SerializeField] BoolVariable SellStationOpen;
    private InventorySlotHandler[] GetInventoryGroup(InventorySlotHandler SlotHandler)
    {
        var Slot = SlotHandler.slotData;
        //SlotType represents the slot type of the one that was clicked
        if (Slot.slotType == InventorySlot.SlotType.Hotbar || Slot.slotType == InventorySlot.SlotType.Backback)
        {
            #region Check for open inventories
            //Priority Top-Bottom

            //Chest is open
            if (ChestOpen.Value)
            {
                return ChestUI.Instance.ChestSlotsHandlers;
            }

            //Tile Maker open
            if (TileMakerOpen.Value)
            {

            }



            //Move to closer place in 
            #endregion
            if (Slot.slotType == InventorySlot.SlotType.Hotbar)
            {
                return inventoryManager.InventorySlotsHandlers;
            }
            return inventoryManager.HotBarSlotsHandlers;
        }
        if (Slot.slotType == InventorySlot.SlotType.Chest)
        {
            //Return player inventory
            return inventoryManager.AllPlayerInventorySlots;

        }
        if (Slot.slotType == InventorySlot.SlotType.ResourceBag)
        {
            if (BankOpen.Value)
            {
                return ResourceBankHandler.Instance.AllBankSlots;
            }

            if (SellStationOpen.Value)
            {
                return new InventorySlotHandler[] { SellStationHandler.Instance.ItemSlot };
            }


            return inventoryManager.ResourceSlotsHandlers;



        }
        if (Slot.slotType == InventorySlot.SlotType.Bank)
        {
            return inventoryManager.ResourceSlotsHandlers;
        }
        if (Slot.slotType == InventorySlot.SlotType.Shop)
        {
            return inventoryManager.AllPlayerInventorySlots;
        }

        return null;

    }
}

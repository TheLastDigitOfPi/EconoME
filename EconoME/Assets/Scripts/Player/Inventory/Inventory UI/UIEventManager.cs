using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;


public class UIEventManager : MonoBehaviour
{
    public static UIEventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    [field: SerializeField] public CanvasGroup MouseItemCanvas { get; private set; }
    [field: SerializeField] public TextMeshProUGUI ItemText { get; private set; }
    [field: SerializeField] public Image ItemImageForeground { get; private set; }
    [field: SerializeField] public Image ItemImageBackground { get; private set; }

    [SerializeReference] private Item ItemOnMouse;
    public bool HoldingItem
    {
        get
        {
            if (ItemOnMouse == null)
                return false;
            if (!ItemOnMouse.IsValid(out string Error))
            {
                //Debug.LogWarning(Error);
                return false;
            }
            return true;
        }
    }
    public bool MouseOverUI
    {
        get { return EventSystem.current.IsPointerOverGameObject(); }
    }
    //If MouseOverUI doesn't work this does
    bool mouseOverUI
    {
        get
        {
            var results = new List<RaycastResult>();
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };
            EventSystem.current.RaycastAll(eventData, results);
            if (results.Count > 0)
                return true;
            return false;
        }
    }

    public void ClickedOnTrash()
    {
        //Delete Item in hand if clicked on trash
        if (HoldingItem)
        {
            ItemOnMouse = null;
            UpdateItemUI();
        }
    }

    public void ClickedOnSlot(IAmASlot SlotClicked)
    {
        //If Holding an item, attempt to add the item to the slot or switch items if they cannot stack
        if (HoldingItem)
        {
            if (SlotClicked.AddItem(ItemOnMouse, out bool PartialAddition) && !PartialAddition)
            {
                ItemOnMouse = null;
                UpdateItemUI();
                return;
            }
            if (PartialAddition)
                SlotClicked.SwapItem(ItemOnMouse, out ItemOnMouse);
            UpdateItemUI();
            return;
        }
        //Otherwise Attempt to grab from the slot
        if (SlotClicked.GrabItem(out var grabbedItem))
        {
            ItemOnMouse = grabbedItem;
        }
        UpdateItemUI();
    }

    void DropItem()
    {
        //Item dropped into world
        if (!HoldingItem) { return; }

        var mousePos = (Input.mousePosition);
        mousePos.z = Camera.main.nearClipPlane;
        var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;
        GroundItemManager.Instance.SpawnItem(ItemOnMouse, worldPos, out _);
        ItemOnMouse = null;
        UpdateItemUI();
    }

    void UpdateItemUI()
    {
        ItemImageForeground.sprite = HoldingItem ? ItemOnMouse.ItemBase.ForegroundIcon.Icon == null ? null : ItemOnMouse.ItemBase.ForegroundIcon.Icon : null;
        ItemImageBackground.sprite = HoldingItem ? ItemOnMouse.ItemBase.BackgroundIcon.Icon == null ? null : ItemOnMouse.ItemBase.BackgroundIcon.Icon : null;

        ItemText.text = HoldingItem ? ItemOnMouse.Stacksize > 1 ? ItemOnMouse.Stacksize.ToString() : default : default;
        ItemImageForeground.color = HoldingItem ? ItemOnMouse.ItemBase.ForegroundIcon.Icon == null ? Color.clear : ItemOnMouse.ItemBase.ForegroundIcon.IconColor : Color.clear;
        ItemImageBackground.color = HoldingItem ? ItemOnMouse.ItemBase.BackgroundIcon.Icon == null ? Color.clear : ItemOnMouse.ItemBase.BackgroundIcon.IconColor : Color.clear;
        MouseItemCanvas.alpha = HoldingItem ? 1 : 0;
    }

    private void Update()
    {
        if (HoldingItem)
        {
            transform.position = Mouse.current.position.ReadValue() + new Vector2(15, 15);
            if (Mouse.current.IsPressed())
            {
                if (!MouseOverUI)
                    DropItem();
            }
        }

    }

}

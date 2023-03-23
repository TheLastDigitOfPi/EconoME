using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using UnityEngine.EventSystems;

public class InventorySlotHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IAmASlot
{
    //Handles the visual display of items as well as interactions player makes with the inventory (Got Clicked)

    [field: Space(10)]
    [field: Header("Inventory Group")]
    [field: SerializeField] public InventoryObject Inventory { get; private set; }
    [field: SerializeField] public int SlotNumber { get; private set; }
    ItemBase ItemBase { get { return ItemSlot.ItemBase; } }
    public ItemSlot ItemSlot { get { return Inventory.Data.ItemSlots[SlotNumber]; } }

    [Space(10)]
    [Header("Visuals")]
    [SerializeField] TextMeshProUGUI itemCountText;
    [SerializeField] Image _foregroundImage;
    [SerializeField] Image ItemSlotEffect;
    [SerializeField] Image _backgroundImage;
    [field: SerializeField] public Image BorderImage { get; private set; }
    [SerializeField] GameObject TileInspect;

    [field: Space(10)]
    [field: Header("Admin/Testing")]
    [field: SerializeField] public DefinedScriptableItem ForceSlotItem { get; private set; }


    public void Initialize(int SlotNum, InventoryObject inventory)
    {
        SlotNumber = SlotNum;
        Inventory = inventory;
        //Subscribe to updates on this slot
        Inventory.Data.ItemSlots[SlotNumber].OnItemChange += UpdateSlot;
        UpdateSlot();
    }

    private void OnDestroy()
    {
        Inventory.Data.ItemSlots[SlotNumber].OnItemChange -= UpdateSlot;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Inventory.Data.ItemSlots[SlotNumber].HasItem)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                //Attempt to quick move item
                PlayerInventoryManager.QuickMoveItem(Inventory, SlotNumber);
                return;
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                RemoveItem();
                return;
            }
        }

        UIEventManager.Instance.ClickedOnSlot(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void UpdateSlot()
    {
        if (ItemSlot.HasItem)
        {
            _foregroundImage.color = ItemBase.ForegroundIcon.IconColor;
            _foregroundImage.sprite = ItemBase.ForegroundIcon.Icon;
            
            _backgroundImage.color = ItemBase.BackgroundIcon.IconColor;
            _backgroundImage.sprite = ItemBase.BackgroundIcon.Icon;
            
            itemCountText.text = ItemSlot.StackSize > 1 ? ItemSlot.StackSize.ToString() : default;
            return;
        }
        _foregroundImage.sprite = default;
        _foregroundImage.color = Color.clear;
        _backgroundImage.sprite = default;
        _backgroundImage.color = Color.clear;
        itemCountText.text = default;
    }

    public bool GrabItem(out Item ItemGrabbed)
    {
        ItemGrabbed = null;
        if (ItemSlot.HasItem)
        {
            Inventory.RemoveItemFromSlot(SlotNumber, out ItemGrabbed);
            return true;
        }

        return false;
    }

    public bool AddItem(Item ItemAdded, out bool PartialAddition)
    {
        return Inventory.AddItemToSlot(ItemAdded, SlotNumber, out PartialAddition);
    }

    public bool RemoveItem()
    {
        Inventory.RemoveItemFromSlot(SlotNumber, out _);
        return true;
    }

    public bool SwapItem(Item ItemToSwap, out Item SwappedItem)
    {
        return Inventory.SawpItem(ItemToSwap, out SwappedItem, SlotNumber);
    }
}

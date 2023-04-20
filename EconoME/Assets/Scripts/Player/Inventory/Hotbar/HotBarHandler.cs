using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInventoryManager))]
public class HotBarHandler : MonoBehaviour
{
    //Static
    public static HotBarHandler Instance { get; private set; }

    //Events
    public event Action<Item> OnSelectItem;
    public event Action<Item> OnDeselectItem;

    //Public
    public int SelectedHotBarSlot { get; private set; } = -1;

    //Local
    [SerializeField] InventoryObject HotBar;
    InputAction _hotBar1;
    InputAction _hotBar2;
    InputAction _hotBar3;
    InputAction _hotBar4;
    InputAction _hotBar5;
    InputAction _hotBar6;
    InputAction _hotBar7;


    Color HighlightedColor = new Color(138, 255, 0);


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 Hot bar handler found!");
            Destroy(this);
            return;
        }
        Instance = this;
        _hotBar1 = CustomInputManager.Instance.Hotbar1; 
        _hotBar2 =CustomInputManager.Instance.Hotbar2;
        _hotBar3 =CustomInputManager.Instance.Hotbar3;
        _hotBar4 =CustomInputManager.Instance.Hotbar4;
        _hotBar5 =CustomInputManager.Instance.Hotbar5;
        _hotBar6 =CustomInputManager.Instance.Hotbar6;
        _hotBar7 =CustomInputManager.Instance.Hotbar7;
    }

    #region Event Subscribing

    private void OnEnable()
    {
        _hotBar1.performed += SelectHotbar1;
        _hotBar2.performed += SelectHotbar2;
        _hotBar3.performed += SelectHotbar3;
        _hotBar4.performed += SelectHotbar4;
        _hotBar5.performed += SelectHotbar5;
        _hotBar6.performed += SelectHotbar6;
        _hotBar7.performed += SelectHotbar7;
    }

    private void OnDisable()
    {
        _hotBar1.performed -= SelectHotbar1;
        _hotBar2.performed -= SelectHotbar2;
        _hotBar3.performed -= SelectHotbar3;
        _hotBar4.performed -= SelectHotbar4;
        _hotBar5.performed -= SelectHotbar5;
        _hotBar6.performed -= SelectHotbar6;
        _hotBar7.performed -= SelectHotbar7;
    }

    private void SelectHotbar1(InputAction.CallbackContext obj) { SelectHotBarSlot(0); }
    private void SelectHotbar2(InputAction.CallbackContext obj) { SelectHotBarSlot(1); }
    private void SelectHotbar3(InputAction.CallbackContext obj) { SelectHotBarSlot(2); }
    private void SelectHotbar4(InputAction.CallbackContext obj) { SelectHotBarSlot(3); }
    private void SelectHotbar5(InputAction.CallbackContext obj) { SelectHotBarSlot(4); }
    private void SelectHotbar6(InputAction.CallbackContext obj) { SelectHotBarSlot(5); }
    private void SelectHotbar7(InputAction.CallbackContext obj) { SelectHotBarSlot(6); }
    #endregion

    void DeselectSlot(int slotNum)
    {
        if(slotNum < 0)
            return;
        var hotBarSlot = PlayerInventoryManager.Instance.HotBarSlotsHandlers[slotNum].ItemSlot;
        ToggleHotBarHighlight(slotNum, ToggleOn: false);
        if (hotBarSlot.HasItem)
            OnDeselectItem?.Invoke(hotBarSlot.Item);
    }
    void SelectSlot(int slotNum)
    {
        SelectedHotBarSlot = slotNum;
        var hotBarSlot = PlayerInventoryManager.Instance.HotBarSlotsHandlers[slotNum].ItemSlot;
        ToggleHotBarHighlight(slotNum, ToggleOn: true);
        if (hotBarSlot.HasItem)
        {
            OnSelectItem?.Invoke(hotBarSlot.Item);
            PlayerInventoryManager.Instance.HotBarSlotsHandlers[slotNum].ItemSlot.OnItemChange += DeselectCurrentSlot; ;
        }
    }

    private void DeselectCurrentSlot()
    {
        DeselectSlot(SelectedHotBarSlot);
    }

    public static bool GetCurrentSelectedItem(out Item foundItem)
    {
        foundItem = null;
        if (Instance.SelectedHotBarSlot == -1)
            return false;
        if (Instance.HotBar.Data.ItemSlots[Instance.SelectedHotBarSlot].HasItem)
        {
            foundItem = Instance.HotBar.Data.ItemSlots[Instance.SelectedHotBarSlot].Item;
            return true;
        }
        return false;
    }

    void SelectHotBarSlot(int newSelectedSlot)
    {
        Debug.Log("Selecting Slot: " + newSelectedSlot);
        if(HeldItemHandler.Instance.UsingItem)
            return;
        //Return If we try to select a slot outside of the hotbar range
        if (newSelectedSlot > HotBar.Data.ItemSlots.Length)
            return;

        //Deselect our old slot
        DeselectSlot(SelectedHotBarSlot);

        //If selected current slot, stop
        if (newSelectedSlot == SelectedHotBarSlot)
        {
            SelectedHotBarSlot = -1;
            return;
        }

        //Otherwise select the new slot
        SelectSlot(newSelectedSlot);
    }

    void ToggleHotBarHighlight(int slotNum, bool ToggleOn = true)
    {
        PlayerInventoryManager.Instance.BackpackHotBarSlotsHandlers[slotNum].BorderImage.color = ToggleOn ? HighlightedColor : Color.white;
        PlayerInventoryManager.Instance.HotBarSlotsHandlers[slotNum].BorderImage.color = ToggleOn ? HighlightedColor : Color.white;
    }

}

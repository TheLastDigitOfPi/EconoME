using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInventoryManager))]
public class HotBarHandler : MonoBehaviour
{
    //Static
    public static HotBarHandler Instance { get; private set; }

    //Events
    public event Action OnItemSelect;
    public event Action OnItemDeselect;

    //Public
    public int SelectedHotBarSlot { get; private set; } = -1;

    //Local
    [SerializeField] InventoryObject HotBar;
    int _previouslySelectedSlot = -1;

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
        OnItemDeselect += DeselectItem;
        OnItemSelect += SelectItem;
    }
    private void OnDestroy()
    {
        OnItemDeselect -= DeselectItem;
        OnItemSelect -= SelectItem;
    }

    private void SelectItem()
    {
        //Try deselect previous slot 
        if (_previouslySelectedSlot > -1)
        {
            ToggleHotBarHighlight(_previouslySelectedSlot, ToggleOn: false);
        }
        //Select New Hotbar Slot
        ToggleHotBarHighlight(SelectedHotBarSlot);
    }

    void DeselectItem()
    {
        ToggleHotBarHighlight(SelectedHotBarSlot, ToggleOn: false);
        SelectedHotBarSlot = -1;
        _previouslySelectedSlot = -1;
    }
    public static bool GetCurrentSelectedItem(out Item foundItem)
    {
        foundItem = null;
        if (Instance.SelectedHotBarSlot == -1)
            return false;
        if (Instance.HotBar.Data.ItemSlots[Instance.SelectedHotBarSlot].HasItem)
        {
            foundItem = Instance.HotBar.Data.ItemSlots[Instance.SelectedHotBarSlot].ItemCopy;
            return true;
        }
        return false;
    }

    void SelectHotBarSlot(int i)
    {
        _previouslySelectedSlot = SelectedHotBarSlot;
        SelectedHotBarSlot = i;

        //Return If we try to select a slot outside of the hotbar range
        if (i > HotBar.Data.ItemSlots.Length)
        {
            SelectedHotBarSlot = -1;
            return;
        }

        //If selected current slot, deselect it
        if (SelectedHotBarSlot == _previouslySelectedSlot)
        {
            OnItemDeselect?.Invoke();
            return;
        }

        OnItemSelect?.Invoke();
        return;
    }

    void ToggleHotBarHighlight(int slotNum, bool ToggleOn = true)
    {
        PlayerInventoryManager.Instance.BackpackHotBarSlotsHandlers[slotNum].BorderImage.color = ToggleOn ? HighlightedColor : Color.white;
        PlayerInventoryManager.Instance.HotBarSlotsHandlers[slotNum].BorderImage.color = ToggleOn ? HighlightedColor : Color.white;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectHotBarSlot(0); ;
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectHotBarSlot(1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectHotBarSlot(2);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectHotBarSlot(3);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectHotBarSlot(4);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectHotBarSlot(5);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectHotBarSlot(6);
            return;
        }
    }


}

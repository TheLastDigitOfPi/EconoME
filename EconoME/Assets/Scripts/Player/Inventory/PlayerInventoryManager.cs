using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PlayerInventoryManager : MonoBehaviour
{
    [Space(10)]
    [Header("Slot Handlers")]
    [SerializeField] InventorySlotHandler[] _resourceSlotHandlers;
    [SerializeField] InventorySlotHandler[] _inventorySlotHandlers;
    [SerializeField] InventorySlotHandler[] _hotBarSlotHandlers;
    [SerializeField] InventorySlotHandler[] _armorSlotHandlers;
    [SerializeField] InventorySlotHandler[] _allPlayerInventoryHandlers;
    [SerializeField] InventorySlotHandler[] _bottomHotbarSlots;

    public InventorySlotHandler[] ResourceSlotsHandlers { get { return _resourceSlotHandlers; } }
    public InventorySlotHandler[] InventorySlotsHandlers { get { return _inventorySlotHandlers; } }
    public InventorySlotHandler[] BackpackHotBarSlotsHandlers { get { return _hotBarSlotHandlers; } }
    public InventorySlotHandler[] HotBarSlotsHandlers { get { return _bottomHotbarSlots; } }
    public InventorySlotHandler[] ArmorSlotsHandlers { get { return _armorSlotHandlers; } }
    public InventorySlotHandler[] AllPlayerInventorySlots { get { return _allPlayerInventoryHandlers; } }

    [Space(10)]
    [Header("Scriptable Object Inventories")]
    [SerializeField] InventoryObject ResourceSlots;
    [SerializeField] InventoryObject BackpackSlots;
    [SerializeField] InventoryObject HotBarSlots;
    [SerializeField] InventoryObject ArmorSlots;

    public HotBarHandler HotBarHandler { get; private set; }

    public static PlayerInventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one player inventory manager found");
            Destroy(this);
            return;
        }
        Instance = this;
        HotBarHandler = GetComponent<HotBarHandler>();

        _defaultInventories.Add(ArmorSlots);
        _defaultInventories.Add(ResourceSlots);
        _defaultInventories.Add(BackpackSlots);
        _defaultInventories.Add(HotBarSlots);
    }

    private void Start()
    {

        InitializeSlots(ResourceSlots, _resourceSlotHandlers);
        InitializeSlots(BackpackSlots, _inventorySlotHandlers);
        InitializeSlots(HotBarSlots, _hotBarSlotHandlers);
        InitializeSlots(ArmorSlots, _armorSlotHandlers);
        InitializeSlots(HotBarSlots, _bottomHotbarSlots);

        UpdateSlots(ResourceSlots, _resourceSlotHandlers);
        UpdateSlots(BackpackSlots, _inventorySlotHandlers);
        UpdateSlots(HotBarSlots, _hotBarSlotHandlers);
        UpdateSlots(ArmorSlots, _armorSlotHandlers);
        UpdateSlots(HotBarSlots, _bottomHotbarSlots);
        
        void InitializeSlots(InventoryObject slots, InventorySlotHandler[] handlers)
        {
            //Make sure data is valid and reset any leftover events from previous plays
            slots.InitializeData();

            if (slots.Data.ItemSlots.Length > handlers.Length)
                Debug.LogWarning("Not Enough Item Slots to Represent the Data for the Items");
        }

        void UpdateSlots(InventoryObject slots, InventorySlotHandler[] handlers)
        {
            //Initialize the UI slots to update on events
            for (int i = 0; i < slots.Data.ItemSlots.Length; i++)
            {
                handlers[i].Initialize(i, slots);
            }
        }


    }

    public static bool AddItemToPlayer(Item GroundItem)
    {
        if (GroundItem.ItemBase is ResourceBase)
        {
            return Instance.ResourceSlots.AddItem(GroundItem);
        }
        //Attempt to add to slots with this item already to items that can stack
        if (GroundItem.IndividualItemWeight > 0)
        {
            if (Instance.HotBarSlots.AddItem(GroundItem, AddToEmpty: false))
                return true;
            if (Instance.BackpackSlots.AddItem(GroundItem, AddToEmpty: false))
                return true;
        }
        //Otherwise add to an empty slot Prioritize adding to backpack first
        if (Instance.BackpackSlots.AddItem(GroundItem, CheckStacks: false))
            return true;

        return Instance.HotBarSlots.AddItem(GroundItem, CheckStacks: false);
    }

    [SerializeField] HashSet<InventoryObject> _activeInventories = new();
    [SerializeField] HashSet<InventoryObject> _defaultInventories = new();
    public static void QuickMoveItem(InventoryObject originalInventory, int slotNum)
    {
        List<InventoryObject> inventoriesToCheck = new();
        //Prioritize Hotbar slots to go to backback slots in default list
        if(originalInventory == Instance.HotBarSlots)
            inventoriesToCheck.Add(Instance.BackpackSlots);
        else if(originalInventory == Instance.BackpackSlots)
            inventoriesToCheck.Add(Instance.HotBarSlots);

        //Add activeInventoriesToList
        foreach (var inventory in Instance._activeInventories)
        {
            if(inventoriesToCheck.Contains(inventory))
                continue;
            inventoriesToCheck.Add(inventory);
        }

        //Add default inventories to list
        foreach (var inventory in Instance._defaultInventories)
        {
            if(inventoriesToCheck.Contains(inventory))
                continue;
            inventoriesToCheck.Add(inventory);
        }

        //Check in the defined inventories
        foreach (var inventory in inventoriesToCheck)
        {
            if (AttemptMoveToInventory(inventory))
                return;
        }

        bool AttemptMoveToInventory(InventoryObject newInventory)
        {
            if (newInventory.ItemRequirement.isValidItem(originalInventory.Data.ItemSlots[slotNum].ItemCopy))
            {
                //Take item from slot and move it to the other inventory. If it doesn't fully fit, place item back in original slot
                originalInventory.RemoveItemFromSlot(slotNum, out Item foundItem);
                if (!newInventory.AddItem(foundItem))
                {
                    originalInventory.AddItemToSlot(foundItem, slotNum, out _);
                }
                return true;
            }
            return false;
        }
    }

}


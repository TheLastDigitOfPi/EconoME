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
    public InventorySlotHandler[] HotBarSlotsHandlers { get { return _hotBarSlotHandlers; } }
    public InventorySlotHandler[] ArmorSlotsHandlers { get { return _armorSlotHandlers; } }
    public InventorySlotHandler[] AllPlayerInventorySlots { get { return _allPlayerInventoryHandlers; } }

    [Space(10)]
    [Header("Scriptable Object Inventories")]
    [SerializeField] InventoryObject ResourceSlots;
    [SerializeField] InventoryObject BackpackSlots;
    [SerializeField] InventoryObject HotBarSlots;
    [SerializeField] InventoryObject ArmorSlots;

    public HotBarHandler HotBarHandler { get; private set; }
    private void Awake()
    {
        HotBarHandler = GetComponent<HotBarHandler>();
        ItemOnMouse itemOnMouse = GetComponentInChildren<ItemOnMouse>();

        InitializeScriptableInventoryObjects();

        void InitializeScriptableInventoryObjects()
        {
            setSlots(ResourceSlots, _resourceSlotHandlers);
            setSlots(BackpackSlots, _inventorySlotHandlers);
            setSlots(HotBarSlots, _hotBarSlotHandlers);
            setSlots(ArmorSlots, _armorSlotHandlers);
            setSlots(HotBarSlots, _bottomHotbarSlots);

            void setSlots(InventoryObject slots, InventorySlotHandler[] handlers)
            {
                if (slots.data.items == null || slots.data.items.Length < handlers.Length)
                {
                    slots.data.items = new Item[handlers.Length];
                }

                for (int i = 0; i < handlers.Length; i++)
                {
                    if (slots.data.items[i] == null)
                    {
                        slots.data.items[i] = new();
                    }

                    handlers[i].slotData.slotIndex = i;
                    handlers[i].owner = itemOnMouse;
                    handlers[i].UpdateSlot();
                }
            }
        }
    }

    public bool AddItemToPlayer(Item GroundItem)
    {
        if (GroundItem.itemType is ResourceType)
        {
            return InventoryManager.AddToInventoryGroup(ResourceSlotsHandlers, GroundItem);
        }
        return InventoryManager.AddToInventoryGroup(AllPlayerInventorySlots, GroundItem);
    }

    public bool AddItemToPlayer(DefinedScriptableItem data)
    {
        return AddItemToPlayer(data.item.CreateItem(data.stacksize));
    }



}


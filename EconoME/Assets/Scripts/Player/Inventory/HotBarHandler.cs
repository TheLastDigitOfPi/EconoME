using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInventoryManager))]
public class HotBarHandler : MonoBehaviour
{
    [SerializeField] InventoryObject HotBar;
    public int SelectedHotBarSlot { get; private set; } = -1;
    [SerializeField] SpriteRenderer ItemInHand;

    PlayerInventoryManager _inventoryManager;
    Color HighlightedColor = new Color(138, 255, 0);
    private void Awake()
    {
        _inventoryManager = GetComponent<PlayerInventoryManager>();
    }

    public bool SelectedHotBarTypeEquals(ItemType type)
    {
        if (SelectedHotBarSlot < 0) { return false; }
        if (HotBar.data.items[SelectedHotBarSlot] == null) { return false; }
        return HotBar.data.items[SelectedHotBarSlot].itemType == type;
    }

    public Item GetSelectedHotBarItem()
    {
        if (SelectedHotBarSlot < 0) { return null; }
        return HotBar.data.items[SelectedHotBarSlot];
    }

    public TextureGroup[] SelectedHotbarAnimationSet
    {
        get
        {
            Item currentItem = GetSelectedHotBarItem();
            if(currentItem is Tool)
            {
                return (currentItem as Tool).ToolSwingAnimations;
            }
            return null;
        }
    }

    void SelectHotBarSlot(int i)
    {
        int PrevSelected = SelectedHotBarSlot;
        SelectedHotBarSlot = i;

        if (i > HotBar.data.items.Length)
        {
            SelectedHotBarSlot = -1;
            return;
        }

        //Deselect Hotbar slot
        if (SelectedHotBarSlot == PrevSelected)
        {
            ToggleHotBarHighlight(SelectedHotBarSlot);
            SelectedHotBarSlot = -1;
            ItemInHand.sprite = null;
            ItemInHand.color = Color.clear;
            return;
        }
        if (PrevSelected > -1)
        {
            //Deselct Previous Hotbar slot
            ToggleHotBarHighlight(PrevSelected);
        }
        //Select New Hotbar Slot
        ToggleHotBarHighlight(SelectedHotBarSlot);
        //Add Item to player hands
        UpdateHotBarSlot();
        return;
    }
    void ToggleHotBarHighlight(int i)
    {
        if (i > _inventoryManager.HotBarSlotsHandlers.Length) { return; }
        if (_inventoryManager.HotBarSlotsHandlers[i].BorderImage.color == Color.white)
        {
            _inventoryManager.HotBarSlotsHandlers[i].BorderImage.color = HighlightedColor;
            return;
        }
        _inventoryManager.HotBarSlotsHandlers[i].BorderImage.color = Color.white;
    }

    public void UpdateHotBarSlot()
    {
        Sprite ItemImage = null;
        if (HotBar.data.items[SelectedHotBarSlot] != null)
        {
            ItemImage = HotBar.data.items[SelectedHotBarSlot].Icon;
        }
        if (ItemImage != null)
        {
            if (ItemImage.texture.width > 16)
            {
                float Newsize = 1f / (ItemImage.texture.width / 16);
                ItemInHand.transform.localScale = new Vector3(Newsize, Newsize, 1);
            }
            else
            {
                ItemInHand.transform.localScale = new Vector3(1f, 1f, 1);
            }
            ItemInHand.color = Color.white;
            ItemInHand.sprite = ItemImage;
        }
        else
        {
            ItemInHand.color = Color.clear;
            ItemInHand.sprite = null;
        }
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
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SelectHotBarSlot(7);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SelectHotBarSlot(8);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SelectHotBarSlot(9);
            return;
        }
    }


}

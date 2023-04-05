using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventorySlotBackground : MonoBehaviour
{
    InventorySlotHandler handler;
    [SerializeField] Image backgroundImage;
    [SerializeField] Sprite backgroundSprite;

    private void Awake()
    {
        handler = GetComponent<InventorySlotHandler>();
        handler.Inventory.Data.ItemSlots[handler.SlotNumber].OnItemChange += UpdateBackground;

        backgroundImage.sprite = backgroundSprite;
        backgroundImage.color = Color.white;
    }

    private void UpdateBackground()
    {
        if (handler.Inventory.Data.ItemSlots[handler.SlotNumber].HasItem)
        {
            backgroundImage.color = Color.white;
            return;
        }
        backgroundImage.color = Color.clear;
        return;

    }
    private void OnDestroy()
    {
        if (handler == null)
            return;
        handler.Inventory.Data.ItemSlots[handler.SlotNumber].OnItemChange -= UpdateBackground;
    }
}

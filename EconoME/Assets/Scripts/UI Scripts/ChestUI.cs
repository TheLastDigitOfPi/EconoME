using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChestUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public InventorySlotHandler[] ChestSlotsHandlers;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform ChestInventoryPos;
    ChestInventoryManager _currentOpenChest;
    public ChestInventoryManager CurrentOpenChest { get { return _currentOpenChest; } }

    public static ChestUI Instance;
    public bool HoldingChestInventory;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        Instance = this;
    }
    [SerializeField] BoolVariable ChestOpen;
    public void openChest(ChestInventoryManager Chest)
    {
        canvasGroup.ToggleCanvas();

        _currentOpenChest = Chest;
        ChestOpen.Value = true;
        if (Chest.data.ChestInvSlots.Length > ChestSlotsHandlers.Length) { Debug.LogWarning("Attempted to add larger chest inventory data then UI had space for"); return; }

        for (int i = 0; i < ChestSlotsHandlers.Length; i++)
        {
            ChestSlotsHandlers[i].slotData = Chest.data.ChestInvSlots[i];
            ChestSlotsHandlers[i].UpdateSlot();
        }
    }

    public void closeChest()
    {
        canvasGroup.ToggleCanvas();

        ChestOpen.Value = false;
        if (CurrentOpenChest.data.ChestInvSlots.Length > ChestSlotsHandlers.Length) { Debug.LogWarning("Attempted to largers chest inventory data then UI had space for"); return; }

        for (int i = 0; i < ChestSlotsHandlers.Length; i++)
        {
            CurrentOpenChest.data.ChestInvSlots[i].SetSlot(ChestSlotsHandlers[i].slotData.item, null);
        }

        _currentOpenChest = null;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out InventorySlotHandler IS))
        {
            return;
        }
        HoldingChestInventory = true;
        StartCoroutine(DragInventory());
    }

    IEnumerator DragInventory()
    {

        do
        {
            Vector3 tempVec = Mouse.current.position.ReadValue();
            Vector3 tempVec3 = new
                Vector3(tempVec.x - 50,
                        tempVec.y - 120,
                        0);
            ChestInventoryPos.position = tempVec3;
            yield return null;
        } while (HoldingChestInventory);


    }

    public void OnPointerUp(PointerEventData eventData)
    {
        HoldingChestInventory = false;
    }
}

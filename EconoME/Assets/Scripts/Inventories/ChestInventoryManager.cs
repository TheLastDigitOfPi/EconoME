using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChestInventoryManager : MonoBehaviour
{
    [SerializeField] ChestUI ChestUI;
    CanvasGroup ChestUICanvas;
    public ChestInstanceData data = new ChestInstanceData();
    Transform Slotholder;
    private void Awake()
    {
        ChestUI = GameObject.FindWithTag("ChestUI").GetComponent<ChestUI>();
        data.position = transform.position;
        for (int i = 0; i < data.ChestInvSlots.Length; i++)
        {
            data.ChestInvSlots[i].slotType = InventorySlot.SlotType.Chest;
        }

    }

    public void OpenChest()
    {
        ChestUI.openChest(this);
    }

    public void setInstanceData(ChestInstanceData other)
    {
        data = other;
    }


}
[Serializable]
public class ChestInstanceData
{
    public Vector3 position;
    public int SlotCount;
    public InventorySlot[] ChestInvSlots;

    public ChestInstanceData()
    {
        ChestInvSlots = new InventorySlot[35];
    }
}
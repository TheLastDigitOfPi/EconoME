using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEditor;
using System.IO;
using UnityEngine.EventSystems;

public class InventorySlotHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    //The slot data for current inventory slot
    //Takes itemClass object data to display in inventory

    //Semi-Public Data. TODO Make Private
    [SerializeReference] public InventorySlot slotData = new();
    [Space(10)]
    //Private Data
    public TextMeshProUGUI text;
    public Image image;

    public Image BorderImage;
    [SerializeField] GameObject TileInspect;
    [SerializeField] DefinedScriptableItem forceSlotItem;
    public DefinedScriptableItem ForceSlotItem {get{return forceSlotItem;}}

    Image InspectObject;
    bool isHovered = false;
    bool tileOpen = false;

    public ItemOnMouse owner;

    private void Update()
    {
        if (Input.GetKey(KeyCode.V))
        {
            if (tileOpen) { return; }
            if (isHovered && slotData.item is WorldTile)
            {
                tileOpen = true;
                InspectObject = Instantiate(TileInspect).GetComponentInChildren<Image>();
                InspectObject.sprite = image.sprite;
            }
        }
        else if (tileOpen)
        {
            tileOpen = false;
            Destroy(InspectObject.transform.parent.gameObject);
        }

    }

    private void Start()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        owner.ClickedOnSlot(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }

    public bool SetSlot(Item item)
    {
        return slotData.SetSlot(item, this);
    }
    public void ClearSlot()
    {
        slotData.ClearSlot(this);
    }

    public void UpdateSlot()
    {
        slotData.UpdateSlot(this);
    }

    public void GrabItem(out Item itemToChange)
    {
        slotData.GrabItem(this, out itemToChange);
    }




}

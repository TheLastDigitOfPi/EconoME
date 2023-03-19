using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ResourceBankHandler : MonoBehaviour
{
    public InventorySlotHandler[] AllBankSlots;
    List<InventorySlotHandler> ActiveBankSlots = new List<InventorySlotHandler>();
    [SerializeField] string Name = "";
    private ResourceType Type;
    [SerializeField] int Rarity = -1;
    [SerializeField] CanvasGroup BankCanvasGroup;
    [SerializeField] CanvasGroup SearchUI;

    [SerializeField] ResourceType Any;
    [SerializeField] ResourceType Mineral;
    [SerializeField] ResourceType Lumber;
    [SerializeField] ResourceType FarmItem;
    [SerializeField] ResourceType MonsterDrop;
    [SerializeField] InventoryObject _bankInventoryData;

    public static ResourceBankHandler Instance;

    void Awake()
    {
        ActiveBankSlots = AllBankSlots.ToList();
        if (Instance != null)
        {
            Debug.LogWarning("Attempted to create more than 1 instance of ResoureBankHandler");
            Destroy(this);
        }
        Instance = this;
        _bankInventoryData.InitializeData();

        if (_bankInventoryData.Data.ItemSlots.Length > AllBankSlots.Length)
            Debug.LogWarning("Not Enough Item Slots to Represent the Data for the Items");

        //Initialize the UI slots to update on events
        for (int i = 0; i < AllBankSlots.Length; i++)
        {
            AllBankSlots[i].Initialize(i, _bankInventoryData);
        }


    }
    /*
    public void SetType(int inputType)
    {
        switch (inputType)
        {
            case 0:
                Type = Any;
                break;
            case 1:
                Type = Mineral;
                break;
            case 2:
                Type = Lumber;
                break;
            case 3:
                Type = FarmItem;
                break;
            case 4:
                Type = MonsterDrop;
                break;
            default:
                break;
        }
        SetAllActiveForSearch();
        StartSearch();
    }

    public void SetRarity(int rarity)
    {
        Rarity = rarity - 1;
        SetAllActiveForSearch();
        StartSearch();
    }

    public void setSearch(string Search)
    {
        if (Name.Contains(Search))
        {
            SetAllActiveForSearch();
        }
        Name = Search;
        StartSearch();
    }
    
    public void StartSearch()
    {

        for (int i = 0; i < ActiveBankSlots.Count; i++)
        {
            var BankSlot = ActiveBankSlots[i].SlotData as BankInventorySlot;
            Resource SlotResouce = ActiveBankSlots[i].SlotData.item as Resource;
            if (Type != Any)
            {

                if (SlotResouce.itemType != Type)
                {
                    AllBankSlots[BankSlot.SlotNumber].SlotData.Active = false;
                    ActiveBankSlots.Remove(ActiveBankSlots[i]);
                    i--;
                    continue;
                }
            }

            if (Rarity != -1)
            {
                if (SlotResouce.Tier != Rarity)
                {
                    AllBankSlots[BankSlot.SlotNumber].SlotData.Active = false;
                    ActiveBankSlots.Remove(ActiveBankSlots[i]);
                    i--;
                    continue;
                }
            }


            if (Name != "")
            {
                if (!SlotResouce.ItemName.ToLower().Contains(Name.ToLower()))
                {
                    AllBankSlots[BankSlot.SlotNumber].SlotData.Active = false;
                    ActiveBankSlots.Remove(ActiveBankSlots[i]);
                    i--;
                    continue;
                }
            }


        }
        RefineSearch();
    }
    
    public void ResetSearch()
    {
        Name = "";
        Type = Any;
        Rarity = -1;
        ActiveBankSlots.Clear();
        for (int i = 0; i < AllBankSlots.Length; i++)
        {
            AllBankSlots[i].gameObject.SetActive(true);
            AllBankSlots[i].SlotData.Active = true;
            ActiveBankSlots.Add(AllBankSlots[i]);
        }
    }

    void SetAllActiveForSearch()
    {
        ActiveBankSlots.Clear();
        for (int i = 0; i < AllBankSlots.Length; i++)
        {
            AllBankSlots[i].gameObject.SetActive(true);
            AllBankSlots[i].SlotData.Active = true;
            ActiveBankSlots.Add(AllBankSlots[i]);
        }
    }

    public void TempResetSearch()
    {
        for (int i = 0; i < AllBankSlots.Length; i++)
        {
            AllBankSlots[i].gameObject.SetActive(true);
        }
    }

    public void RefineSearch()
    {
        for (int i = 0; i < AllBankSlots.Length; i++)
        {
            if (!AllBankSlots[i].SlotData.Active)
            {
                AllBankSlots[i].gameObject.SetActive(false);
            }
        }
    }
    */
    public void ToggleSearchTab()
    {
        SearchUI.alpha = SearchUI.alpha == 1 ? 0 : 1;
        SearchUI.interactable = !SearchUI.interactable;
        SearchUI.blocksRaycasts = !SearchUI.blocksRaycasts;
    }
    [SerializeField] BoolVariable BankOpen;
    public void ToggleUI()
    {
        BankCanvasGroup.ToggleCanvas();
    }
}

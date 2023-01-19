using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventorySlot : InventorySlot
{
    [SerializeField] bool ShopStation;
    [SerializeField] IntVariable PlayerCurrency;
    [SerializeField] IntVariable TileSaveNumber;
    [SerializeField] ShopTradeObject shopTrade;
    [SerializeField] ItemType ResourceItemType;
    public ShopInventorySlot() : base()
    {
        slotType = SlotType.Shop;
    }

    public override bool SetSlot(Item NewItem, InventorySlotHandler slotHandler)
    {
        if(ShopStation)
        {
            item = NewItem;
            if(SellStationHandler.Instance.SellItem(slotHandler))
            {
                return true;
            }
        }
        return false;
    }

    public override bool CanGrabSlot()
    {
        if(!base.CanGrabSlot()){return false; }
        if(shopTrade.Cost > PlayerCurrency.Value){return false;}

        return true;

    }

    public override void GrabItem(InventorySlotHandler HandlerGrabbedFrom, out Item newItemSpot)
    {
        if(item is WorldTile)
        {
           TileSaveNumber.Value++;
            (item as WorldTile).TileName = "Test" + TileSaveNumber.Value;
            string datapath = Application.dataPath + "/TileScreenshots/" + (item as WorldTile).TileName + "Screenshot.png";
            //Move File
            if (System.IO.File.Exists(datapath))
            {
                System.IO.File.Delete(datapath);
            }
            System.IO.File.Move((item as WorldTile).IconPath, datapath);
            item.SetIcon(item.Icon, datapath);
        }
        base.GrabItem(HandlerGrabbedFrom, out newItemSpot);
    }

    public void AttemptTrade(PlayerInventoryManager owner)
    {
        if (shopTrade.RemainingTrades < 1) { return; }
        if (PlayerCurrency.Value < shopTrade.Cost) { return; }

        List<DefinedScriptableItem> ResourceItems = shopTrade.RequiredItems.FindAll(a=> a.item.ItemType is ResourceType);
        List<DefinedScriptableItem> NonResourceItems = shopTrade.RequiredItems.FindAll(a=> a.item.ItemType is not ResourceType);

        if(!InventoryManager.RemoveItems(ResourceItems, owner.ResourceSlotsHandlers))
        {
            Debug.Log("Rip items lol");
        }
        if(!InventoryManager.RemoveItems(shopTrade.RequiredItems, owner.AllPlayerInventorySlots))
        {
            Debug.Log("Rip items lol");
        }

        //Add items to player
        owner.AddItemToPlayer(shopTrade.PurcahseItem);
        PlayerCurrency.Value -= shopTrade.Cost;
        return;
    }

}

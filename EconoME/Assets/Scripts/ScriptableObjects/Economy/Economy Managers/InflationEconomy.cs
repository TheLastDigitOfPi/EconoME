using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Inflation Based Economy", menuName = "ScriptableObjects/Economy/Economy Managers/Infaltion Based Economy")]
public class InflationEconomy : EconomyManager
{
    public override int GetSellPrice(Item itemSold)
    {
        return SellPrice(itemSold, false);
    }

    int SellPrice(Item itemSold, bool SellItem)
    {
        //Find item
        ItemBase foundItem = EconomyItems.FirstOrDefault(item => item.ItemName == itemSold.ItemName);
        if (foundItem == null) { return -1; }

        //Get inflation price
        int ItemPrice = foundItem.GetInflationPrice(itemSold.Stacksize, SellItem);
        
        //Apply multipliers
        ItemPrice = (int)(ItemPrice * foundItem.Multipler);
        
        //Return final price
        return ItemPrice;
    }

    public override int Sell(Item itemSold)
    {
        return SellPrice(itemSold, true);
    }
}

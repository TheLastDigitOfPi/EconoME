using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class EconomyManager : ScriptableObject
{
    [SerializeField] ItemScriptableObject[] economyItems;
    public ItemScriptableObject[] EconomyItems{ get { return economyItems;} }
    public abstract int GetSellPrice(Item itemSold);
    public abstract int Sell(Item itemSold);

}

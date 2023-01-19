using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public abstract class ItemScriptableObject : ScriptableObject
{
    [SerializeField] string _itemName;
    [SerializeField] public Sprite Icon;
    [SerializeField] string _iconPath;
    [SerializeField] int _weight;
    public abstract ItemType ItemType { get; }
    public string ItemName { get { return _itemName; } }
    public string IconPath { get { return _iconPath; } }
    public int Weight { get { return _weight; } }

    [SerializeField] EconomyStatus EconomyData = new();

    public int GetInflationPrice(int amountSold, bool sellItems)
    {
        return EconomyData.GetInflationPrice(amountSold, sellItems);
    }

    public virtual Item CreateItem(int stackSize)
    {
        return new Item(this, stackSize);
    }

    public float Multipler
    {
        get
        {
            if (EconomyData.multiplier == null) { return 1; }
            return EconomyData.multiplier.Multiplier;
        }
    }

}

[Serializable]
public class EconomyStatus
{
    [SerializeField] int BasePrice;
    [SerializeField] int Threshold = 1;
    [SerializeField] int RemainingThreshold = 100;
    [SerializeField] int DailyInflationHits;
    [SerializeField] int DailyItemsSold;

    [SerializeField] float InflationPerHit = 0.02f;
    [SerializeField] int TotalInflationHits;
    [SerializeField] int MaxInflationHits = 25;
    [SerializeField] int TotalItemsSold;
    public ItemMultiplier multiplier;


    int CurrentInflationHits
    {
        get
        {
            if ((DailyInflationHits + TotalInflationHits) > MaxInflationHits)
            {
                return MaxInflationHits;
            }
            else
            {
                return DailyInflationHits + TotalInflationHits;
            }
        }
    }

    public int GetInflationPrice(int amountSold, bool SellItems)
    {
        int remainingHolder = RemainingThreshold;
        int dailyHitsHolder = DailyInflationHits;
        int totalHitsHolder = TotalInflationHits;

        int TotalBasePrice = 0;
        //Check if selling these items will break the threshold and force tax increase
        if (amountSold >= RemainingThreshold)
        {
            //Remove items sold 
            amountSold -= RemainingThreshold;

            //Get Price from remaning threshold section
            TotalBasePrice += (int)((BasePrice * RemainingThreshold) * (1 - ((CurrentInflationHits) * InflationPerHit)));
            DailyInflationHits++;
            RemainingThreshold = Threshold;

            if (amountSold > RemainingThreshold)
            {
                int totalThresholdHits = amountSold / Threshold;
                int totalItemsSold = (int)(BasePrice * totalThresholdHits * Threshold);
                float AvgInterestRate = 0;
                for (int i = 0; i < totalThresholdHits; i++)
                {
                    AvgInterestRate += 1 - (InflationPerHit * (CurrentInflationHits));
                }
                AvgInterestRate /= TotalInflationHits;
                TotalBasePrice += Mathf.RoundToInt((totalItemsSold * AvgInterestRate));
                DailyInflationHits += totalThresholdHits;
                amountSold -= totalThresholdHits * Threshold;
            }
            RemainingThreshold -= amountSold;

            //Add remaining stack amount
            TotalBasePrice += (int)((BasePrice * amountSold) * (1 - ((CurrentInflationHits) * InflationPerHit)));
        }
        else
        {
            //Add remaining stack amount
            TotalBasePrice += (int)((BasePrice * amountSold) * (1 - ((CurrentInflationHits) * InflationPerHit)));
            RemainingThreshold -= amountSold;
        }

        if (!SellItems)
        {
            RemainingThreshold = remainingHolder;
            DailyInflationHits = dailyHitsHolder;
            TotalInflationHits = totalHitsHolder;
        }

        return TotalBasePrice;
    }





}

[CreateAssetMenu(fileName = "New Item Multiplier", menuName = "ScriptableObjects/Economy/Item Multiplier")]
public class ItemMultiplier : ScriptableObject
{
    public float Multiplier;
}


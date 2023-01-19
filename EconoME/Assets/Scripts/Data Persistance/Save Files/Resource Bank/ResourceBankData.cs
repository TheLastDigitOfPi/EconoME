using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class ResourceBankData
{ 
    public List<int> BankSlotsItemCount;
    public List<string> BankSlotNames;

    public ResourceBankData()
    {
        BankSlotsItemCount = new List<int>();
        BankSlotNames = new List<string>();
    }
   
}

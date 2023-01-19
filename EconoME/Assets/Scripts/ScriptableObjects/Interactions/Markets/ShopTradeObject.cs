using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Trade", menuName = "ScriptableObjects/Interactions/Markets/Shops Trade")]
public class ShopTradeObject : ScriptableObject
{
    public DefinedScriptableItem PurcahseItem;
    public List<DefinedScriptableItem> RequiredItems;
    public int Cost;
    public int RemainingTrades;

}

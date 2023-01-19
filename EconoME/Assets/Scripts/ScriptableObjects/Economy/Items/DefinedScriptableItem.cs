using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Defined Item ", menuName = "ScriptableObjects/Economy/Items/Defined Item")]
public class DefinedScriptableItem : ScriptableObject
{
    public ItemScriptableObject item;
    public int stacksize;

    public Item CreateItem()
    {
        return item.CreateItem(stacksize);
    }
}

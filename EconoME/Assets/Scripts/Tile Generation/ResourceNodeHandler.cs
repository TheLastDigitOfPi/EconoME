using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceNodeHandler : Raycastable
{
    // Resource Node Script:
    // Represents a Resource Node placed in the world (tree, rock, crop, etc)

    [SerializeField] ResourceSriptableObject resource;
    [SerializeField] GameObject WorldItemPrefab;
    public ResourceNode data;
    public void Start()
    {
        if (data.PopThreshold == 0)
        {
            data.PopThreshold = data.health / (data.MaxPops != 0? data.MaxPops : 1);
        }
        data.RemainingThreshold = data.PopThreshold;
    }

    //Checks if current tool is correct type and tier
    bool CheckIfCorrectTool(ToolType ToolUsed, int TierUsed)
    {
        if (ToolUsed != data.ToolNeeded || TierUsed < data.Tier)
        {
            return false;
        }
        return true;
    }
    //Called when correct tool it used on node
    public void Hit(Item testItem)
    {
        if (testItem == null) { return; }
        if (testItem is not Tool) { return; }
        Tool ToolUsed = testItem as Tool;
        if(ToolUsed.itemType is not ToolType){return;}
        if (!CheckIfCorrectTool((ToolUsed.itemType as ToolType), ToolUsed.ToolTier))
        {
            return;
        }

        //play harvesting animation

        //Check number of items to drop
        int RequestedPops = 0;
        int TempDamge = ToolUsed.Damage;
        while (data.health > 0 && TempDamge > 0)
        {
            if (TempDamge > data.RemainingThreshold)
            {
                RequestedPops++;
                TempDamge -= data.RemainingThreshold;
                data.health -= data.RemainingThreshold;
                data.RemainingThreshold = data.PopThreshold;
            }
            else
            {
                data.RemainingThreshold -= TempDamge;
                data.health -= TempDamge;
                TempDamge = 0;
            }
        }
        data.PopsLeft -= RequestedPops;
        for (int i = 0; i < RequestedPops; i++)
        {
            //Drop Item
            DropItem();
        }
        //Health - Damage of tool
        if (data.health <= 0)
        {
            Break();
        }
    }


    //Resource Node Breaks
    void Break()
    {
        //play break animation
    }
    [ContextMenu("DropItem")]
    void DropItem()
    {
        GameObject NewItemDrop = Instantiate(WorldItemPrefab);
        WorldItemHandler WUI = NewItemDrop.GetComponent<WorldItemHandler>();
        NewItemDrop.transform.position = transform.position;


        WUI.data.item = new Resource(resource);
        WUI.data.item.Stacksize = data.DroppedItemsPerPop;
        WUI.updateImage();
        WUI.updateText();

        NewItemDrop.GetComponent<BounceInteraction>().StartBounce();

    }

    public override bool OnRaycastHit(PlayerController owner, Collider2D collider)
    {
        Debug.Log(name + " was hit by a raycast!");
        if (owner.InventoryManager.HotBarHandler.SelectedHotBarTypeEquals(data.ToolNeeded) && !owner.UsingTool)
        {
            owner.StartUsingTool();
            owner.onUseTool += () => Hit(owner.InventoryManager.HotBarHandler.GetSelectedHotBarItem());
        }
        return true;
    }
}



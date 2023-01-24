using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceNodeHandler : MonoBehaviour, IAmInteractable
{
    // Resource Node Script:
    // Represents a Resource Node placed in the world (tree, rock, crop, etc)



    //Public fields
    [field: SerializeField] public ResourceNode NodeData { get; private set; }
    [field: SerializeField] public ResourceBase NodeItem { get; private set; }

    //Events
    public event Action OnNodeHit;
    public event Action OnNodeDestroy;

    //private serialized fields
    [SerializeField] GameObject WorldItemPrefab;
    [SerializeField] ParticleSystem BreakAnimation;
    [SerializeField] ParticleSystem HitAnimation;
    [SerializeField] NodeIndicator IndicatorPrefab;
    [SerializeField] Transform IndicatorPosition;
    [SerializeField] Vector3 animationOffset = new Vector3(0f, 0.2f);

    //local fields
    GameObject parent;
    NodeIndicator indicator;
    public void Start()
    {
        parent = transform.parent.gameObject;
    }

    //Called when correct tool it used on node
    public void Hit(ToolBase toolBase)
    {


        if (!NodeData.AttemptHit(toolBase, out List<int> dropCounts, out bool brokeNode))
        {
            return;
        }
        //play harvesting animation
        var animation = Instantiate(HitAnimation, transform);
        animation.transform.position += animationOffset;


        OnNodeHit?.Invoke();

        for (int i = 0; i < dropCounts.Count; i++)
        {
            //Drop Items
            DropItem(dropCounts[i]);
        }
        //Health - Damage of tool
        if (brokeNode)
        {
            Break();
        }
        void DropItem(int itemCount)
        {
            if (GroundItemManager.Instance.SpawnItem(NodeItem.CreateItem(itemCount), transform.position, out WorldItemHandler handler))
            {
                handler.GetComponent<BounceInteraction>().StartBounce();
            }
        }
    }


    //Resource Node Breaks
    void Break()
    {
        var animation = Instantiate(BreakAnimation, transform);
        animation.transform.position += animationOffset;
        OnNodeDestroy?.Invoke();
        Destroy(parent, animation.main.duration / animation.main.simulationSpeed);
    }

    public void HideIndicator()
    {
        Destroy(indicator.gameObject);
        indicator = null;
    }

    public bool OnRaycastHit(PlayerMovementController owner, Collider2D collider)
    {
        Debug.Log(gameObject.name + " was hit by a raycast!");

        if (HotBarHandler.GetCurrentSelectedItem(out Item foundItem))
        {

            //Validate the interaction

            if (!CheckIfValidTool(out ToolBase foundTool))
                return false;

            if (!NodeData.ValidNode)
                return false;

            if (!owner.StartUsingTool(NodeData.TimeToHarvest / foundTool.Speed))
                return false;

            if (indicator == null)
            {
                indicator = Instantiate(IndicatorPrefab, IndicatorPosition);
                indicator.Initialize(this);
            }


            owner.OnEndUseTool += HitNode;

            bool CheckIfValidTool(out ToolBase foundTool)
            {
                foundTool = null;
                if (foundItem.ItemBase is not ToolBase)
                    return false;

                foundTool = foundItem.ItemBase as ToolBase;

                if (foundTool.ToolType != NodeData.ToolNeeded)
                    return false;

                return true;
            }
            void HitNode()
            {
                Hit(foundItem.ItemBase as ToolBase);
                owner.OnEndUseTool -= HitNode;
            }
        }



        return true;
    }

}



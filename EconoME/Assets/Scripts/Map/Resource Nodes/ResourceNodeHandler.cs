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

    //Events
    public event Action OnNodeHit;
    public event Action OnNodeDestroy;

    //fields to set in editor

    [Header("Particle Effects")]
    [SerializeField] ParticleSystem BreakAnimation;
    [SerializeField] ParticleSystem HitAnimation;
    [SerializeField] Vector3 animationOffset = new Vector3(0f, 0.2f);

    [Header("Indicator")]
    [SerializeField] NodeIndicator IndicatorPrefab;
    [SerializeField] Transform IndicatorPosition;
    [field: SerializeField] public Sprite IndicatorIcon { get; private set; }

    //local fields
    GameObject parent;
    NodeIndicator indicator;

    //Helpers
    Item NodeItem => NodeData.NodeBase.ResourceDropTable.GetDrop().CreateItem();

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
            if (GroundItemManager.Instance.SpawnItem(NodeItem, transform.position, out WorldItemHandler handler))
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
            HeldItemHandler.Instance.OnComplete += HitNode;
            HeldItemHandler.Instance.StartProgress(NodeData.TimeToHarvest / foundTool.Speed);
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
                HeldItemHandler.Instance.OnComplete -= HitNode;
            }
        }



        return true;
    }

}



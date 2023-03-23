using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Updates the UI for an item that is currently on the ground in the world
/// </summary>
public class WorldItemHandler : MonoBehaviour
{
    //Public data
    [field: SerializeField] public WorldItemInstance data { get; private set; } = new WorldItemInstance();
    public bool SetForRemoval { get; private set; } = false;
    public bool PickupByPlayer { get; set; } = false;

    //Local fields
    //Assign in editor
    [SerializeField] Image _itemForegroundImage;
    [SerializeField] Image _itemBackgroundImage;
    [SerializeField] Image _itemEffectImage;
    [SerializeField] TextMeshProUGUI _itemCountText;
    [SerializeField] bool AllowCombining = false;

    PickupInteraction pickupHandler;

    //Helpers
    public bool CanCombine { get { return !SetForRemoval && pickupHandler.CanBePickedUp && !PickupByPlayer; } }

    private void Start()
    {
        if (data.itemPreset != null)
        {
            data.item = data.itemPreset.CreateItem();
            UpdateImage();
            UpdateText();
        }
        pickupHandler = GetComponent<PickupInteraction>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!CanCombine)
            return;

        WorldItemHandler foundHandler;
        if (!collision.TryGetComponent(out foundHandler))
            return;

        if (!foundHandler.CanCombine)
            return;

        if (foundHandler.data.item.ItemBase != data.item.ItemBase)
            return;

        CombineItems(foundHandler);
    }

    void CombineItems(WorldItemHandler other)
    {
        data.item.Stacksize += other.data.item.Stacksize;
        UpdateText();
        GroundItemManager.Instance.ItemRemoved(other);
        other.data.item.Stacksize = 0;
        other.SetForRemoval = true;
    }

    public void UpdateImage()
    {
        if (data.item == null)
        {
            Debug.LogWarning("Tried to update world item image but the item data was never instantiated");
            return;
        }
        if (data.item.ForegroundIcon != null)
        {
            _itemForegroundImage.sprite = data.item.ForegroundIcon.Icon;
            _itemForegroundImage.color = data.item.ForegroundIcon.IconColor;
            _itemBackgroundImage.sprite = data.item.BackgroundIcon.Icon;
            _itemBackgroundImage.color = data.item.BackgroundIcon.IconColor;
            return;
        }

        _itemForegroundImage.sprite = default;
        _itemForegroundImage.color = Color.clear;
        _itemBackgroundImage.sprite = default;
        _itemBackgroundImage.color = Color.clear;

    }

    internal void CreateItem(Item item, Vector3 position)
    {
        data.item = item.Duplicate();
        data.WorldPos = position;
        UpdateImage();
        UpdateText();
        transform.position = position;
    }

    public void UpdateText()
    {
        //updateUI of text element on ground object

        if (data.item == null)
        {
            Debug.LogWarning("Tried to update world item text but the item data was never instantiated");
            return;
        }
        _itemCountText.text = data.item.Stacksize > 1 ? data.item.Stacksize.ToString() : default;
    }

    public void DestroyItem()
    {
        GroundItemManager.Instance.ItemRemoved(this);
    }

    internal void setInstanceData(WorldItemInstance other)
    {
        data = other;
        UpdateText();
        UpdateImage();
    }
}


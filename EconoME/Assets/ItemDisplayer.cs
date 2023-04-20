using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ItemDisplayer : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    [SerializeField] Transform holder;
    [SerializeField] TextMeshProUGUI _text;
    [SerializeField] Image backgroundImage;
    [SerializeField] Image foregroundImage;
    [SerializeField] Vector3 offScreenPosition;
    [SerializeField] Vector3 centerScreenPosition;
    [SerializeField] bool canCancel;

    public event Action OnComplete;

    Item _itemToSpawn;

    public void Initialize(Item item)
    {
        _itemToSpawn = item;
        canCancel = false;
        _text.text = item.Stacksize > 1 ? item.Stacksize.ToString() : default;
        backgroundImage.sprite = item.BackgroundIcon.Icon;
        backgroundImage.color = item.BackgroundIcon.IconColor;
        foregroundImage.sprite = item.ForegroundIcon.Icon;
        foregroundImage.color = item.ForegroundIcon.IconColor;

        holder.position = offScreenPosition;

        holder.DOLocalMove(centerScreenPosition, 1f).SetEase(Ease.InOutSine).SetUpdate(true).onComplete += OnFinishMoving;

        void OnFinishMoving()
        {
            canCancel = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canCancel)
            return;
        if (PlayerInventoryManager.AddItemToPlayer(_itemToSpawn))
        {
            holder.position = offScreenPosition;
            OnComplete?.Invoke();
            Destroy(this.gameObject);
            return;
        }
        if (!GroundItemManager.Instance.SpawnItem(_itemToSpawn, PlayerMovementController.Instance.PlayerPosition.Value, out _))
            return;
        holder.position = offScreenPosition;
        OnComplete?.Invoke();
        Destroy(this.gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Needed to enable pointer behavior
    }

}
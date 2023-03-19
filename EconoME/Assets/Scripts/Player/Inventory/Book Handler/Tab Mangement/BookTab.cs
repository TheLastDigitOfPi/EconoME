using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;
using UnityEngine.InputSystem;

public class BookTab : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [field: SerializeField] public Image TabImage { get; private set; }

    public event Action<UnlockableInventory> OnClick;

    public float InitialX;
    float MoveDistance { get { return _manager.MoveDistance; } }
    float MovedX { get { return InitialX + MoveDistance; } }

    [SerializeField] float TimeToPickupTab = 0.75f;
    private bool playerReleasedMouse;
    public UnlockableInventory Inventory { get; private set; }
    BookTabManager _manager;
    bool mouseOverTab;

    public void Initialize(UnlockableInventory inventory, BookTabManager bookTabManager)
    {
        _manager = bookTabManager;
        Inventory = inventory;
        TabImage.sprite = inventory.TabSprite;
        var rotation = TabImage.transform.localRotation;
        if (inventory.TabSide == TabSide.Left)
            rotation.z = 180;
        else
            rotation.z = 0;
        TabImage.transform.localRotation = rotation;

    }

    private void Start()
    {
        InitialX = (transform as RectTransform).localPosition.x;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Start timer for if going to hold tab
        playerReleasedMouse = false;
        StartCoroutine(WaitForChoice());
        IEnumerator WaitForChoice()
        {
            yield return new WaitForSeconds(TimeToPickupTab);
            if (!playerReleasedMouse && mouseOverTab)
            {
                //Create a new object that is held by the player and can be dropped onto either tab side
                var heldTab = Instantiate(_manager.HeldTabPrefab, _manager.transform);
                heldTab.Initialize(Inventory, _manager);
                _manager.RemoveTab(this);
            }
            playerReleasedMouse = false;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Open Tab to Side
        mouseOverTab = true;
        transform.DOLocalMoveX(MovedX, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Close Tab back
        mouseOverTab = false;
        transform.DOLocalMoveX(InitialX, 0.2f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        playerReleasedMouse = true;
        StopAllCoroutines();
        if (mouseOverTab)
            OnClick?.Invoke(Inventory);

    }
}

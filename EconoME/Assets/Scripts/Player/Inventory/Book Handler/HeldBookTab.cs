using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HeldBookTab : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    [SerializeField] Image _tabImage;

    BookTabManager bookTabManager;
    RectTransform LeftTabsTransform { get { return bookTabManager.LeftTabsParent.transform as RectTransform; } }
    RectTransform RightTabsTransform { get { return bookTabManager.RightTabsParent.transform as RectTransform; } }

    [SerializeField] Vector3 _holdingOffSet;

    RectTransform rectTransform { get { return (transform as RectTransform); } }
    private UnlockableInventory _inventory;

    public void Initialize(UnlockableInventory inventory, BookTabManager tabManager)
    {
        bookTabManager = tabManager;
        _inventory = inventory;
        _tabImage.sprite = inventory.TabSprite;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Let Go of Tab
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 finalPosition = mousePos + _holdingOffSet;
        rectTransform.position = finalPosition;

        if (finalPosition.x < Screen.currentResolution.width/2)
            _inventory.TabSide = TabSide.Left;
        if (finalPosition.x >= Screen.currentResolution.width/2)
            _inventory.TabSide = TabSide.Right;

        bookTabManager.AddTab(_inventory);
        Destroy(gameObject);

    }

    private void Update()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 finalPosition = mousePos + _holdingOffSet;
        rectTransform.position = finalPosition;
    }

    public bool InsideArea(RectTransform transform1, RectTransform transform2)
    {
        var area1 = transform1.GetWorldRect();
        var area2 = transform2.GetWorldRect();

        return area1.xMin <= area2.xMin && area1.yMin <= area2.yMin && area1.xMax >= area2.xMax && area1.yMax >= area2.yMax;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Required so that other pointer handler will go off, who knows why :(
        return;
    }
}


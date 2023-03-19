using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class BookPage : MonoBehaviour
{
    [SerializeField] string _pageTitle;
    [SerializeField] TextMeshProUGUI _toolTipText;
    [SerializeField][TextArea] string[] _toolTips;
    string ToolTip { get { return _toolTips.RandomItem(); } }
    [SerializeField] Sprite _icon;

    [field: SerializeField] public UnlockableInventory Inventory { get; private set; }

    PlayerBookHandler _bookHandler;
    public void Initialize(PlayerBookHandler handler)
    {
        _bookHandler = handler;
        if (Inventory == null)
            return;
        Inventory.ConnectedPage = this;
    }

    private void OnDestroy()
    {
        Inventory.ConnectedPage = null;
    }

    public void SetPage(Transform parentTransform, Image PageIcon, TextMeshProUGUI TitleText)
    {
        gameObject.SetActive(true);
        transform.SetParent(parentTransform, false);
        PageIcon.sprite = _icon;
        TitleText.text = _pageTitle;
        if (_toolTipText != null)
        {
            _toolTipText.text = ToolTip;
        }
        (transform as RectTransform).ForceUpdateRectTransforms();
    }

}


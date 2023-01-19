using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BookPage : MonoBehaviour
{
    [SerializeField] string _pageTitle;
    [SerializeField] TextMeshProUGUI _toolTipText;
    [SerializeField][TextArea] string[] _toolTips;
    string ToolTip { get { return _toolTips.RandomItem(); } }
    [SerializeField] Sprite _icon;

    public void SetPage(Transform LeftPageHolder ,Image PageIcon, TextMeshProUGUI TitleText)
    {
        gameObject.SetActive(true);
        transform.SetParent(LeftPageHolder);
        PageIcon.sprite = _icon;
        TitleText.text = _pageTitle;
        if (_toolTipText != null)
        {
            _toolTipText.text = ToolTip;
        }
        (transform as RectTransform).ForceUpdateRectTransforms();
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class UIButtonColorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Color originalTextColor;
    TextMeshProUGUI tmp;
    [SerializeField] Color ColorToChangeTo;
    private void Start()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        originalTextColor = tmp.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tmp.color = ColorToChangeTo;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tmp.color = originalTextColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UITextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] RectTransform Text;
    [SerializeField] float SizeChange  = 1.2f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Text.localScale *= SizeChange;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Text.localScale /= SizeChange;
    }
}

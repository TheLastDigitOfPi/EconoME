using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UITextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] RectTransform Text;
    [SerializeField] float SizeChange = 1.2f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Text.localScale *= SizeChange;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Text.localScale /= SizeChange;
    }

    private void Start()
    {
        //Vector3 startingPos = transform.localPosition;
        //transform.DOLocalMoveY(startingPos.y +30f, 3f).SetEase(Ease.InOutSine).SetLoops(-1,LoopType.Yoyo);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UITweenAnimation : MonoBehaviour
{

    RectTransform UITransform;
    [SerializeField] float _cycleLength = 2;
    [SerializeField] float MoveToYValue;
    // Start is called before the first frame update
    void Start()
    {
        UITransform = GetComponent<RectTransform>();
        UITransform.DOLocalMoveY(MoveToYValue, _cycleLength).SetEase(Ease.InOutSine).SetLoops(-1,LoopType.Yoyo);
    }
}

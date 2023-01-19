using UnityEngine;
using DG.Tweening;


/*
 Pops up UI "tabs" like the X button to close the UI when the player hovers over it
 Slowly migrating most UI's to be in the book, but script may be usefull in future
 */
public class UIPopInAndOutAnimation : MonoBehaviour
{

    RectTransform UITransform;
    public float CycleLength = 2;
    public float MoveToYValue;
    public float InitialYValue;
    // Start is called before the first frame update
    void Start()
    {
        UITransform = GetComponent<RectTransform>();
    }

    public void MoveToInitial()
    {
        UITransform.DOKill();
        UITransform.DOLocalMoveY(InitialYValue, CycleLength).SetEase(Ease.InOutSine);
    }

    public void MoveToNew()
    {
        UITransform.DOKill();
        UITransform.DOLocalMoveY(MoveToYValue, CycleLength).SetEase(Ease.InOutSine);
    }
}
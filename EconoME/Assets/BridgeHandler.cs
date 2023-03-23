using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BridgeHandler : MonoBehaviour
{
    SpriteRenderer _renderer;
    [SerializeField] float dropInDistance = 20;
    [SerializeField] float dropInTime = 1.5f;
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.color = Color.clear;
    }
    public void DropIn(WorldTileHandler handler)
    {
        _renderer.color = Color.white;
        float initialY = transform.localPosition.y;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + dropInDistance, 0);
        transform.DOLocalMoveY(initialY, dropInTime).SetEase(Ease.InOutSine);
    }
}

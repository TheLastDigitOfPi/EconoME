using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class NodeIndicator : MonoBehaviour
{
    [SerializeField] Image _backgroundImage;
    [SerializeField] Image _foregroundImage;
    [SerializeField] float _rotateAmount;

    ResourceNodeHandler nodeHandler;
    public void StartProgress(Sprite icon, float totalTime)
    {
        if (icon == null)
        {
            Destroy(gameObject);
            return;
        }
        _backgroundImage.sprite = icon;
        _foregroundImage.sprite = icon;

        _foregroundImage.fillAmount = 0;
        StartCoroutine(StartFill());
        IEnumerator StartFill()
        {
            float currentTime = 0;
            while (_foregroundImage.fillAmount < 1)
            {
                currentTime += Time.deltaTime;
                _foregroundImage.fillAmount = currentTime / totalTime;
                yield return null;
            }
            PlayerMovementController.Instance.UseTool();
            Destroy(gameObject);
        }
    }

    public void Initialize(ResourceNodeHandler handler)
    {
        Sprite icon = handler.NodeItem.Icon;
        nodeHandler = handler;
        if (icon == null)
        {
            Destroy(gameObject);
            return;
        }
        handler.OnNodeHit += UpdateIndicator;
        handler.OnNodeDestroy += OnNodeDestory;
        _backgroundImage.sprite = icon;
        _foregroundImage.sprite = icon;

        _foregroundImage.fillAmount = nodeHandler.NodeData.RemainingPercentHealth;
    }

    private void OnDestroy()
    {
        nodeHandler.OnNodeHit -= UpdateIndicator;
        nodeHandler.OnNodeDestroy -= OnNodeDestory;
    }

    public void UpdateIndicator()
    {
        StopAllCoroutines();
        transform.DOKill();
        transform.localRotation = Quaternion.identity;
        //Shake animation
        _foregroundImage.fillAmount = nodeHandler.NodeData.RemainingPercentHealth;
        transform.DOShakeRotation(0.8f, new Vector3(0,0,30f), vibrato:0).SetEase(Ease.InOutSine);

        StartCoroutine(HideAfterNoInteraction());
        IEnumerator HideAfterNoInteraction()
        {
            yield return new WaitForSeconds(4f);
            nodeHandler.HideIndicator();
        }
    }

    

    void OnNodeDestory()
    {
        transform.DOKill();
    }
}

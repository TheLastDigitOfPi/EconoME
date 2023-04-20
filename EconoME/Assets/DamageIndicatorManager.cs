using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DamageIndicatorManager : MonoBehaviour
{
    public static DamageIndicatorManager Instance;
    [SerializeField] GameObject IndicatorPrefab;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void CreateIndicator(float changeInHealth, EntityCombatController controller)
    {
        var indicator = Instantiate(IndicatorPrefab);
        var text = indicator.GetComponentInChildren<TextMeshProUGUI>();
        text.text = Mathf.Abs(((int)changeInHealth)).ToString();
        text.color = changeInHealth < 0 ? Color.red : Color.green;
        //Set starting position
        indicator.transform.position = controller.transform.position + Vector3.up * 0.5f;
        
        //Set animation
        var randomUp = Vector3.up * Random.Range(1f, 2f);
        var randomLifetime = Random.Range(1f, 2f);
        var randomX = Vector3.right * Random.Range(-1f, 1f);
        indicator.transform.DOLocalMove(controller.transform.position + randomUp + randomX, randomLifetime).SetEase(Ease.InOutSine).onComplete += () => { Destroy(indicator); };
        

    }
}

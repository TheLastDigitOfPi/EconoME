using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HeldItemHandler : MonoBehaviour
{
    //Static
    public static HeldItemHandler Instance;

    //Events
    public event Action OnComplete;
    public event Action OnStart;

    //Local
    [SerializeField] SpriteRenderer frontIconForeground;
    [SerializeField] SpriteRenderer frontIconBackground;
    [SerializeField] SpriteRenderer backIconForeground;
    [SerializeField] SpriteRenderer backIconBackground;
    [SerializeField] SpriteMask spriteMask;

    Color ORIGINAL_BACK_COLOR = new Color(1, 1, 1, 120 / 255f);
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than 1 held item handler found!");
            Destroy(this);
            return;
        }
        Instance = this;
        frontIconForeground.sprite = default;
        frontIconBackground.sprite = default;
        backIconForeground.sprite = default;
        backIconBackground.sprite = default;
    }

    private void Start()
    {
        HotBarHandler.Instance.OnSelectItem += SelectItem;
        HotBarHandler.Instance.OnDeselectItem += DeSelectItem;
        spriteMask.localBounds = frontIconForeground.localBounds;
    }

    private void OnDestroy()
    {
        HotBarHandler.Instance.OnSelectItem -= SelectItem;
        HotBarHandler.Instance.OnDeselectItem -= DeSelectItem;
    }

    private void DeSelectItem(Item item)
    {

        frontIconForeground.sprite = default;
        frontIconBackground.sprite = default;
        backIconForeground.sprite = default;
        backIconBackground.sprite = default;

        frontIconForeground.color = Color.clear;
        frontIconBackground.color = Color.clear;
        backIconForeground.color = Color.clear;
        backIconBackground.color = Color.clear;
    }

    private void SelectItem(Item foundItem)
    {
        DeSelectItem(foundItem);

        var iconBase = foundItem.ItemBase.ForegroundIcon;
        var icon = iconBase.Icon;


        frontIconForeground.sprite = icon;
        backIconForeground.sprite = icon;
        backIconForeground.color = iconBase.IconColor;
        frontIconForeground.color = iconBase.IconColor;

        if (foundItem.ItemBase.BackgroundIcon.Icon != null)
        {
            var backIconBase = foundItem.ItemBase.BackgroundIcon;
            var backIcon = backIconBase.Icon;

            frontIconBackground.sprite = backIcon;
            backIconBackground.sprite = backIcon;

            frontIconBackground.color = backIconBase.IconColor;
            backIconBackground.color = backIconBase.IconColor;
        }

        Vector2 ScaledObjectValue = new Vector2(16f / (icon.bounds.size.x * icon.pixelsPerUnit), 16f / (icon.bounds.size.y * icon.pixelsPerUnit));

        frontIconForeground.transform.localScale = ScaledObjectValue;
        backIconForeground.transform.localScale = ScaledObjectValue;

        frontIconBackground.transform.localScale = ScaledObjectValue;
        backIconBackground.transform.localScale = ScaledObjectValue;

        spriteMask.transform.localScale = ScaledObjectValue / 2;

    }

    public void StartProgress(float totalTime)
    {
        OnStart?.Invoke();
        spriteMask.transform.localPosition = new Vector3(-spriteMask.transform.localScale.x, 0, 0);
        StartCoroutine(StartFill());
        IEnumerator StartFill()
        {
            float currentTime = 0;
            while (spriteMask.transform.localPosition.x < 0)
            {
                currentTime += Time.deltaTime;
                spriteMask.transform.localPosition = new Vector3(-spriteMask.transform.localScale.x + currentTime / totalTime, 0, 0);
                yield return null;
            }
            OnComplete?.Invoke();
        }
    }
}

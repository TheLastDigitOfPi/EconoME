using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HeldItemHandler : MonoBehaviour
{
    //Static
    public static HeldItemHandler Instance;

    //Local
    [SerializeField] SpriteRenderer frontIcon;
    [SerializeField] SpriteRenderer backIcon;
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
        frontIcon.sprite = default;
        backIcon.sprite = default;
    }

    private void Start()
    {
        HotBarHandler.Instance.OnItemSelect += SelectItem;
        HotBarHandler.Instance.OnItemDeselect += DeSelectItem;
        spriteMask.localBounds = frontIcon.localBounds;
    }

    private void OnDestroy()
    {
        HotBarHandler.Instance.OnItemSelect -= SelectItem;
        HotBarHandler.Instance.OnItemDeselect -= DeSelectItem;
    }

    private void DeSelectItem()
    {
        frontIcon.sprite = default;
        frontIcon.color = Color.clear;
        backIcon.sprite = default;
        backIcon.color = Color.clear;
    }

    private void SelectItem()
    {
        if (HotBarHandler.GetCurrentSelectedItem(out Item foundItem))
        {
            var icon = foundItem.ItemBase.Icon;
            frontIcon.sprite = icon;
            frontIcon.color = Color.white;
            backIcon.sprite = icon;
            backIcon.color = ORIGINAL_BACK_COLOR;

            Vector2 ScaledObjectValue = new Vector2(16 / icon.texture.width, 16 / icon.texture.height);
            frontIcon.transform.localScale = ScaledObjectValue;
            backIcon.transform.localScale = ScaledObjectValue;
            spriteMask.transform.localScale = ScaledObjectValue / 2;

        }
    }

    public void StartProgress(float totalTime)
    {
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
            PlayerMovementController.Instance.UseTool();
        }
    }
}

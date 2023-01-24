using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Collections;

public class SellStationHandler : ShopRaycast
{
    //Get item that is being sold
    public static SellStationHandler Instance;

    [SerializeField] EconomyManager EconomyManager;
    public Sprite SellIcon;
    public InventorySlotHandler ItemSlot;
    [SerializeField] CanvasGroup Toggler;
    [SerializeField] TextMeshProUGUI SellPrice;
    [SerializeField] TextMeshProUGUI PlayerCurrencyText;
    [SerializeField] float PopupTime;
    [SerializeField] float PopupSpeed;
    [SerializeField] BoolVariable SellStationOpen;
    [SerializeField] IntVariable PlayerCurrency;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Attempted to make more than 1 SellStationHanlder Instance");
            Destroy(this.gameObject);
        }
        Instance = this;

    }
    private void Start()
    {
        PlayerCurrency.onValueChange += UpdateCurrency;
        SellStationOpen.onValueChange += ToggleShop;
        PlayerCurrency.Value += 0;
    }

    private void OnDisable()
    {
        PlayerCurrency.onValueChange -= UpdateCurrency;
        SellStationOpen.onValueChange -= ToggleShop;
    }
    /*
    public bool SellItem(InventorySlotHandler handler = null)
    {
        if (handler == null) { handler = ItemSlot; }
        if (handler.SlotData.item == null) { return false; }
        if (handler.SlotData.item.ItemName == null) { Debug.LogWarning("Attempted to sell item with no name"); return false; }
        if (handler.SlotData.item is not Resource) { return false; }

        //Add value to player
        int TotalPrice = EconomyManager.Sell(handler.SlotData.item);

        if (TotalPrice <= 0) { return false; }
        PlayerCurrency.Value += TotalPrice;

        StartCoroutine(SellPopup(TotalPrice));
        //Remove Item
        ItemSlot.ClearSlot();
        return true;
    }
    */
    private void UpdateCurrency()
    {
        PlayerCurrencyText.text = PlayerCurrency.Value.ToString();
    }

    public void ToggleShop()
    {
        Toggler.SetCanvas(SellStationOpen.Value);
    }


    IEnumerator SellPopup(int price)
    {

        TextMeshProUGUI Text = Instantiate(SellPrice, transform);
        Text.text = price.ToString();
        float TimeMoving = PopupTime;
        while (TimeMoving > 0)
        {
            Text.transform.position += Vector3.up * Time.deltaTime * PopupSpeed;
            TimeMoving -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(Text.gameObject);


    }

}

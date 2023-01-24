using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Collections;

public class BarterShopHandler : MonoBehaviour
{
    [SerializeField] List<ShopTradeHandler> tradeList;
    [SerializeField] CanvasGroup Toggler;
    [SerializeField] TextMeshProUGUI MoneyPopup;
    [SerializeField] TextMeshProUGUI PlayerCurrencyText;
    [SerializeField] float PopupTime;
    [SerializeField] float PopupSpeed;
    [SerializeField] IntVariable PlayerCurrency;
    [SerializeField] BoolVariable BarterShopActive;


    IEnumerator SellPopup(int price)
    {

        TextMeshProUGUI Text = Instantiate(MoneyPopup, transform);
        float xOffset = (Text.transform as RectTransform).rect.x * (Text.transform as RectTransform).pivot.x - ((Text.text.Length / 2) * (Text.fontSize / 2));
        float yOffset = (Text.transform as RectTransform).rect.y * (Text.transform as RectTransform).pivot.y - (Text.fontSize / 2);
        Text.transform.position = new Vector3(PlayerCurrencyText.transform.position.x - xOffset, PlayerCurrencyText.transform.position.y - yOffset, 0);
        Text.text = (price * -1).ToString();
        float TimeMoving = PopupTime;
        Vector3 MoveAngle = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(0.2f, 1f));
        while (TimeMoving > 0)
        {
            Text.transform.position += Time.deltaTime * PopupSpeed * (MoveAngle);
            TimeMoving -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(Text.gameObject);
    }

    void updateUI()
    {
        PlayerCurrencyText.text = PlayerCurrency.Value.ToString();
    }

    private void Start()
    {
        PlayerCurrency.onValueChange += updateUI;
        BarterShopActive.onValueChange += ToggleUI;
    }

    private void OnDisable()
    {
        PlayerCurrency.onValueChange -= updateUI;
        BarterShopActive.onValueChange -= ToggleUI;
    }

    public void ToggleUI()
    {
        Toggler.SetCanvas(BarterShopActive.Value);
    }
}



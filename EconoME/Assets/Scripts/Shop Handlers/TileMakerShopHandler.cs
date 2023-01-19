using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class TileMakerShopHandler : MonoBehaviour
{
    public List<InventorySlotHandler> ShopItems = new List<InventorySlotHandler>();
    [SerializeField] TileGeneratorObject tileGenerator;

    private int TileType = 0;
    private int TileTier = 0;

    [SerializeField] TilePlacerObject WorldHandler;
    [SerializeField] TileType TileToMake;

    public void setTileType(int type) { TileType = type; }
    public void setTileTier(int tier) { TileTier = tier; }

    public static TileMakerShopHandler Instance;
    [SerializeField] CanvasGroup canvasGroup;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("Attempted to create more than 1 instance of TileMakerShopHandler");
            Destroy(this.gameObject);
        }
        Instance = this;
    }

    [SerializeField] IntVariable TileMakerMaxTiles;
    public async void RefreshShop()
    {
        for (int i = 0; i < TileMakerMaxTiles.Value; i++)
        {
            var TileToMake = await Task.Run(() =>
            {
                return tileGenerator.GenerateTile(this.TileToMake, 0);
            });
            TileToMake.TileName = "TileMakerTemp" + i;
            var MadeTile = WorldHandler.GetTilePicture(TileToMake);
            ShopItems[i].slotData.item = MadeTile.data;

            if (i == TileMakerMaxTiles.Value - 1)
            {
                MadeTile.OnPictureRetrieved += () =>
                {
                    UpdateShop();
                };
            }
            MadeTile.OnPictureRetrieved += () =>
                {
                    Destroy(MadeTile.gameObject);
                };

        }
        Resources.UnloadUnusedAssets();

    }
    private void UpdateShop()
    {
        for (int i = 0; i < ShopItems.Count; i++)
        {
            ShopItems[i].UpdateSlot();
        }
    }
    [SerializeField] BoolVariable TileMakerOpen;
    public void ToggleShop()
    {
        Debug.Log("Toggle Shop Method");
        TileMakerOpen.Value = !TileMakerOpen.Value;
    }

    public void ToggleShopUI()
    {
        canvasGroup.ToggleCanvas(TileMakerOpen.Value);
    }

    private void Start()
    {
        TileMakerOpen.onValueChange += ToggleShopUI;
    }

    private void OnDisable()
    {
        TileMakerOpen.onValueChange -= ToggleShopUI;
    }



}

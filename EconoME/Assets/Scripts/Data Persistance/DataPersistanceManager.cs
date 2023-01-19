using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistanceManager : MonoBehaviour
{
    //Save and Load Script for all save types
    [SerializeField] private bool useEncryption;

    [SerializeField] private WorldTilesFileHandler WorldSave;
    [SerializeField] private PlayerInventoryFileHandler PlayerInventory;
    [SerializeField] private ChestInventoryFileHandler ChestData;
    [SerializeField] private WorldItemsFileHandler WorldItems;
    [SerializeField] private PlayerDataFileHandler GlobalModifiersData;
    [SerializeField] private ResourceBankFileHandler BankData;

    [SerializeField] private PrefabDataBase prefabs;
    public static DataPersistanceManager instance { get; private set; }
    //Every DataType that has it's own file will need a [DataType]Data class
    //Every [DataType]Data Class needs a FileHandler to post to a unique Text file


    //Data objects that instantiate items (Placed tiles, ground items, interactables, etc)
    //should have only 1 DataObject Interface

    //Data objects that don't require instantiation (inventory slots, player data, events)
    //should each have an interface connected, as well as a Unique ID

    //Each distinct data object type will represent a file to create
    //For example DONT put a gamedata for an individual tile - put one for all tiles

    //Map generator should have TileObject DataObject Class
    //Will take in list of all placed tiles, and their data and place them
    //Or will set all the data so the file handler can post it


    /*
    Data Classes Needed:
    
    X PlacedTile - Implements with MapGenerator 
    X Player Inventory - Implements with inventoryManager
    X ChestInventories - Placed on each chest storage item (Chests need unique ID if static)
    ResourceBank - Placed on ResourceBank
    X WorldItems - Placed on World Item (Placed Item needs unique ID)
    X PlayerStats - Implements with Player

    (Future)
    NPC interaction
    Economy data
    Time(Day/Night Cycle)
    Upgrade Station offers
    Bartering Offers
    Tile Maker Offers


     
     
    */

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in Scene");
        }
        instance = this;

    }
    
    [SerializeField] BoolVariable ChestOpen;

    public void NewGame()
    {
        WorldSave = new WorldTilesFileHandler(useEncryption);
        PlayerInventory = new PlayerInventoryFileHandler();
        ChestData = new ChestInventoryFileHandler();
    }

    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        WorldSave.Save();
        PlayerInventory.Save();

        #region Save Chest Data
        if (ChestOpen.Value)
        {
            ChestUI.Instance.closeChest();
        }
        ChestData.setAllChestData(getAllChestData());
        ChestData.Save();
        #endregion

        WorldItems.setAllItemData(getAllWorldItems());
        WorldItems.Save();

        GlobalModifiersData.Save();
        BankData.Save();

        Debug.Log("Game Saved");
    }

    [ContextMenu("Load Game")]
    public void LoadGame()
    {
        Debug.Log("Loading Game...");
        WorldSave.Load();
        PlayerInventory.Load();
        ChestData.Load();
        PlaceChestData();

        WorldItems.Load();
        PlaceWorldItems();

        GlobalModifiersData.Load();
        BankData.Load();

    }

    [ContextMenu("Test GlobalMods Save")]
    public void SaveGlobalMods()
    {
        GlobalModifiersData.Save();
    }
    [ContextMenu("Test GlobalMods Load")]
    public void LoadGlobalMods()
    {
        GlobalModifiersData.Load();
    }


    public void OnApplicationQuit()
    {
        //SaveGame();
    }


    public List<ChestInstanceData> getAllChestData()
    {
        List<ChestInventoryManager> listOfManagers = FindObjectsOfType<ChestInventoryManager>().ToList();
        List<ChestInstanceData> AllChestData = new List<ChestInstanceData>();
        for (int i = 0; i < listOfManagers.Count; i++)
        {
            AllChestData.Add(listOfManagers[i].data);
        }

        return AllChestData;
    }

    public void PlaceChestData()
    {
        ChestInventoryData data = ChestData.chestData;

        for (int i = 0; i < data.data.Count; i++)
        {
            ChestInventoryManager chestManager;
            chestManager = Instantiate(prefabs.ChestPrefab);
            chestManager.transform.position = data.data[i].position;
             //= PlacedChest.GetComponent<ChestInventoryManager>();
            chestManager.setInstanceData(data.data[i]);
        }
    }

    public List<WorldItemInstance> getAllWorldItems()
    {
        List<WorldItemHandler> ItemsHandler = FindObjectsOfType<WorldItemHandler>().ToList();
        List<WorldItemInstance> AllItems = new List<WorldItemInstance>();
        for (int i = 0; i < ItemsHandler.Count; i++)
        {
            WorldItemInstance itemInstance = new WorldItemInstance();
            itemInstance.item = ItemsHandler[i].data.item;
            itemInstance.WorldPos = ItemsHandler[i].transform.position;
            AllItems.Add(itemInstance);
        }

        return AllItems;
    }

    public void PlaceWorldItems()
    {
        WorldItemsData data = WorldItems.worldItems;
        for (int i = 0; i < data.ItemsData.Count; i++)
        {
            GameObject PlacedItem = Instantiate(prefabs.WorldItemPrefab);
            PlacedItem.transform.position = data.ItemsData[i].WorldPos;
            WorldItemHandler itemHandler = PlacedItem.GetComponent<WorldItemHandler>();
            itemHandler.setInstanceData(data.ItemsData[i]);
        }
    }



}
[Serializable]
public class PrefabDataBase
{
    public GameObject TilePrefab;
    public GameObject WorldItemPrefab;
    public ChestInventoryManager ChestPrefab;

}
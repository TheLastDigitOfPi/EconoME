using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DemoWorldManager : MonoBehaviour
{
    public static DemoWorldManager Instance;

    [SerializeField] TextMeshProUGUI TitleMessage;
    [SerializeField] GameObject ExitDemoButton;
    [SerializeField] NPC NPCHelper;
    [SerializeField] ItemBase GeneralTilebase;
    [SerializeField] List<ResourceNodeSpawnSetting> _possibleSpawnSettings = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        CustomInputManager.Instance.LeftShift.performed += LeftShiftUsed;
        CustomInputManager.Instance.KeyboardC.performed += KeyboardCUsed;
        CustomInputManager.Instance.LeftControl.performed += LeftControlUsed;

    }

    private void LeftShiftUsed(InputAction.CallbackContext obj)
    {
        
    }

    private void KeyboardCUsed(InputAction.CallbackContext obj)
    {

    }
    private void LeftControlUsed(InputAction.CallbackContext obj)
    {
        if(Keyboard.current.periodKey.isPressed)
        { 
            ExitDemoButton.SetActive(!ExitDemoButton.activeSelf);
            return;
        }
        if (Keyboard.current.cKey.isPressed)
        {
            List<ResourceNodeSpawnSetting> nodes = new();
            nodes.Add(_possibleSpawnSettings.RandomListItem());
            TileSettings settings = new(GeneralTilebase ,0, UnsortedExtensions.RandomBiome(), nodes);
            TileItem RandomTile = new(settings);
            PlayerInventoryManager.AddItemToPlayer(RandomTile);
        }
    }

    private void OnDisable()
    {
        CustomInputManager.Instance.LeftShift.performed -= LeftShiftUsed;
        CustomInputManager.Instance.KeyboardC.performed -= KeyboardCUsed;
        CustomInputManager.Instance.LeftControl.performed -= LeftControlUsed;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        CameraManager.Instance.MainCam.gameObject.SetActive(false);
        PlayerMovementController.Instance.gameObject.SetActive(false);
        PlayerInventoryManager.RemoveAllItems();
        PlayerInventoryManager.Instance.gameObject.SetActive(false);
    }

    public async void StartGamePlay()
    {
        PlayerMovementController.Instance.gameObject.SetActive(true);
        PlayerInventoryManager.Instance.gameObject.SetActive(true);
        CameraManager.Instance.MainCam.gameObject.SetActive(true);
        await Task.Delay(500);
        TitleMessage.SetText("Great! Use the WASD keys to move");
        CustomInputManager.Instance.MoveKeys.started += PlayerMoved;
    }

    private async void PlayerMoved(InputAction.CallbackContext obj)
    {
        CustomInputManager.Instance.MoveKeys.started -= PlayerMoved;
        await Task.Delay(500);
        TitleMessage.gameObject.SetActive(false);
        await Task.Delay(1000);
        TitleMessage.text = "A helpful NPC has spawned in! Press space when looking at them to interact with them!";
        TitleMessage.gameObject.SetActive(true);
        NPCHelper.gameObject.SetActive(true);
        NPCHelper.OnStartTalkToPlayer += TalkedToNPC;
        NPCHelper.OnEndTalkToPlayer += FinishedTalkingToPlayer;
    }

    private async void FinishedTalkingToPlayer()
    {
        NPCHelper.OnEndTalkToPlayer -= FinishedTalkingToPlayer;
        TitleMessage.gameObject.SetActive(false);
        await Task.Delay(1000);
        TitleMessage.gameObject.SetActive(true);
        TitleMessage.text = "Toggle open your inventory with 'Tab'";
        CustomInputManager.Instance.Tab.performed += OpenedBook1;

        HotBarHandler.Instance.OnSelectItem += SelectedItem;

        async void OpenedBook1(InputAction.CallbackContext obj)
        {
            CustomInputManager.Instance.Tab.performed -= OpenedBook1;
            CustomInputManager.Instance.Tab.performed += ClosedBook1;
            await Task.Delay(300);
            TitleMessage.text = "Grab the axe by clicking on it, then drop it in your hotbar";
        }
        async void ClosedBook1(InputAction.CallbackContext obj)
        {
            CustomInputManager.Instance.Tab.performed -= ClosedBook1;
            await Task.Delay(300);
            TitleMessage.text = "Select the axe by clicking the number key of the slot it is in";
        }
    }

    private void SelectedItem(Item item)
    {
        if (item.ItemName == "Basic Axe")
        {
            HotBarHandler.Instance.OnSelectItem -= SelectedItem;
            TitleMessage.text = "Great, now use the interact key (Spacebar) on a tree to gather wood!";
            HeldItemHandler.Instance.OnStart += StartedUsingTool;
            HeldItemHandler.Instance.OnComplete += UsedTool;
        }
    }

    private void StartedUsingTool()
    {
        if (HeldItemHandler.Instance.ItemBeingUsed.ItemName != "Basic Axe")
            return;
        TitleMessage.gameObject.SetActive(false);
        HeldItemHandler.Instance.OnStart -= StartedUsingTool;
    }

    private async void UsedTool()
    {
        HeldItemHandler.Instance.OnComplete -= UsedTool;
        await Task.Delay(500);
        TitleMessage.gameObject.SetActive(true);
        TitleMessage.text = "Nice work. Resources are stored in your resource bag.";
        await Task.Delay(6000);
        TitleMessage.text = "To view your resources, open your inventory with 'Tab' and click the resource bag tab on the right";
        CustomInputManager.Instance.Tab.performed += OpenedBook;
    }

    private async void OpenedBook(InputAction.CallbackContext obj)
    {
        CustomInputManager.Instance.Tab.performed -= OpenedBook;
        CustomInputManager.Instance.Tab.performed += ClosedBook;
        await Task.Delay(800);
        TitleMessage.text = "You can close the book at any time by pressing tab again";

        async void ClosedBook(InputAction.CallbackContext obj)
        {
            CustomInputManager.Instance.Tab.performed -= ClosedBook;
            TitleMessage.gameObject.SetActive(false);
            await Task.Delay(1500);
            TitleMessage.gameObject.SetActive(true);
            TitleMessage.text = "Oh no, a hoard of enemies has spawned! Equip your sword and use the interact key to kill them!";
            WorldTileManager.Instance.TryGetTileData(Vector2Int.zero, out var data);

            enemiesKilled = 0;
            for (int i = 0; i < 10; i++)
            {
                var randX = Vector3.right * Random.Range(-0.5f, 0.5f);
                var randY = Vector3.up * Random.Range(-0.5f, 0.5f);
                var randPos = NPCHelper.transform.position + randX + randY;
                var mobToSpawn = data.TileSettings.MobsThatCanSpawn[0];
                var spawnnedMob = Instantiate(mobToSpawn.objectPrefab);
                spawnnedMob.Agent.enabled = false;
                spawnnedMob.transform.position = randPos;
                spawnnedMob.Agent.enabled = true;
                spawnnedMob.Health.OnNoHealth += KilledEnemy;
            }

        }


    }

    public void ReturnToMenu()
    {
        GlobalSceneManager.Instance.ReturnToMainMenu();
    }

    int enemiesKilled = 0;
    private async void KilledEnemy()
    {
        enemiesKilled++;
        if (enemiesKilled < 10)
            return;
        TitleMessage.gameObject.SetActive(false);
        await Task.Delay(500);
        TitleMessage.gameObject.SetActive(true);
        TitleMessage.text = "Nice work, you have completed the tutorial, happy adventuring!";
        ExitDemoButton.gameObject.SetActive(true);
        await Task.Delay(1000);
        ExitDemoButton.SetActive(true);
    }

    private async void TalkedToNPC()
    {
        NPCHelper.OnStartTalkToPlayer -= TalkedToNPC;
        TitleMessage.gameObject.SetActive(false);
        await Task.Delay(1500);
        TitleMessage.gameObject.SetActive(true);
        TitleMessage.text = "To continue a conversation, click on the chat box";

    }
}

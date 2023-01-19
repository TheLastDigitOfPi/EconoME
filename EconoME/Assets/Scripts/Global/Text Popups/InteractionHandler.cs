using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Linq;
using System.Text;

public class InteractionHandler : MonoBehaviour
{
    [Space(10)]
    [Header("Test Interaction")]
    [SerializeField] Interaction TestInteraction;

    [Space(10)]
    [Header("Componenets")]

    [SerializeField] CanvasGroup canvas;
    [SerializeField] Image popupImage;
    [SerializeField] List<Button> _optionsButtons = new();
    [field: SerializeField] public TextMeshProUGUI nameTitleText {get; private set;}
    public List<Button> OptionsButtons { get { return _optionsButtons; } }
    [SerializeField] TextMeshProUGUI _uIText;
    public TextMeshProUGUI UIText { get { return _uIText; } }
    [SerializeField] BoolVariable TextUIActive;

    [Space(10)]
    [Header("Settings")]
    [SerializeField] int _maxCharPerPopup = 300;
    public int MaxCharPerPopup { get { return _maxCharPerPopup; } }
    [SerializeField] int _moveTextSize = 300;
    public int MoveTextSize { get { return _moveTextSize; } }


    [Space(10)]
    [Header("Current State")]
    public bool allTextWritten = true;
    public bool canCancelText;
    public bool nextText = true;
    [Multiline] public List<string> RemainingTexts = new();
    [SerializeField] float timer;
    [SerializeField] int characterIndex;
    Interaction CurrentInteraction;
    public float CurrentTimePerCharacter;
    public string CurrentStringToWrite;
    public Queue<Interaction> Queue { get { return _queue; } }
    Queue<Interaction> _queue = new();
    public Action onFinalText;

    public static InteractionHandler Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one instance of Interaction Handler in scene");
            Destroy(this.gameObject);
        }
        Instance = this;
    }


    private void Update()
    {

        if (Queue.Count == 0 && TextUIActive.Value && nextText && RemainingTexts.Count == 0)
        {
            if (TextUIActive.Value)
            {
                TextUIActive.Value = false;
            }
            return;
        }
        if (Queue.Count > 0 && nextText && RemainingTexts.Count == 0)
        {
            ActivateNextInteraction();
        }

        if (!allTextWritten)
        {
            WriteText();
        }


        void WriteText()
        {
            if (CurrentStringToWrite == "") { allTextWritten = true; return; }
            timer -= Time.deltaTime;
            while (timer <= 0f)
            {
                characterIndex++;
                timer += CurrentTimePerCharacter;
                //Place current written text
                string text = CurrentStringToWrite.Substring(0, characterIndex);
                //Place rest of text as invisible
                text += "<color=#00000000>" + CurrentStringToWrite.Substring(characterIndex) + "</color>";

                _uIText.text = text;

                if (characterIndex >= CurrentStringToWrite.Length)
                {
                    allTextWritten = true;
                    characterIndex = 0;
                    timer = 0;
                    GoToNextText();
                    return;
                }
            }

            void GoToNextText()
            {

                if (RemainingTexts.Count > 0)
                {
                    CurrentStringToWrite = RemainingTexts[0];
                    RemainingTexts.RemoveAt(0);
                }
                else
                {
                    CurrentStringToWrite = "";
                }
            }
        }

        void ActivateNextInteraction()
        {
            CurrentInteraction = Queue.Dequeue();
            if (CurrentInteraction is TextPopup)
            {
                if (!TextUIActive.Value)
                {
                    TextUIActive.Value = true;
                }
            }

            CurrentInteraction.Activate(this);


        }
    }


    [ContextMenu("Test Interaction")]
    void TestTheInteraction()
    {
        Queue.Enqueue(TestInteraction);
    }

    public void AddNew(Interaction newInteraction)
    {
        Interaction[] interactions = Queue.ToArray();
        //Make sure interaction isn't already queued up
        for (int i = 0; i < interactions.Length; i++)
        {
            if (interactions[i].IsEqualTo(newInteraction)) { return; }
        }
        Queue.Enqueue(newInteraction);
    }


    public void PlayerLeftNPC(Guid[] chatIDs)
    {
        if (!TextUIActive.Value) { return; }
        if (chatIDs == null || chatIDs.Length == 0) { return; }
        for (int i = 0; i < chatIDs.Length; i++)
        {
            Guid foundChat;
            Interaction foundInteraction;
            foundInteraction = Queue.FirstOrDefault(p => p.ID == chatIDs[i]);
            foundChat = foundInteraction? foundInteraction.ID : Guid.Empty;
                //Queue.Select(p => p.ID == chatIDs[i]);
            if (foundChat != null)
            {
                _queue = new Queue<Interaction>(_queue.Where(p => p.ID != chatIDs[i]));
                Debug.Log("Found active Interaction with ID of " + foundChat.ToString());
            }

        }
        nextText = true;

    }

    void ToggleUI()
    {
        canvas.ToggleCanvas(TextUIActive.Value);
    }

    private void Start()
    {
        TextUIActive.onValueChange += ToggleUI;
    }
    private void OnDisable()
    {
        TextUIActive.onValueChange -= ToggleUI;
    }


    public void OnClickBackground()
    {
        if (CurrentInteraction is not TextPopup) { Debug.LogWarning("Clicked on Text UI but current interaction was not UI"); return; }
        TextPopup textPopup = CurrentInteraction as TextPopup;
        if (!allTextWritten)
        {
            _uIText.text = CurrentStringToWrite;

            allTextWritten = true;
            characterIndex = 0;
            timer = 0;

            if (RemainingTexts.Count > 0)
            {
                CurrentStringToWrite = RemainingTexts[0];
                RemainingTexts.RemoveAt(0);
            }
            else
            {
                CurrentStringToWrite = "";
            }

            return;
        }
        if (canCancelText)
        {
            if (CurrentStringToWrite == "")
            { nextText = true; }
            allTextWritten = false;
            if (RemainingTexts.Count == 0) { onFinalText?.Invoke(); onFinalText = null; }

        }
    }

    public void DisableOldUI()
    {
        _uIText.margin = Vector4.zero;
        for (int i = 0; i < 9; i++)
        {
            _optionsButtons[i].gameObject.SetActive(false);
        }
        canCancelText = true;
        resetWriter();
        void resetWriter()
        {
            _uIText.text = "";
            nextText = false;
            allTextWritten = false;
            characterIndex = 0;
            timer = 0;
        }
    }
}

[Serializable]
public class Option
{
    public string optionText;
    public UnityAction onSelect;
    public Option(string optionText)
    {
        this.optionText = optionText;
    }

}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;
using UnityEngine.EventSystems;

public class ChatBoxHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{

    //Events
    public event Action OnChatBoxClose;

    //Public Fields
    [field: Space(10)]
    [field: Header("Settings")]
    [field: SerializeField] public int MaxCharPerPopup { get; private set; } = 300;
    [field: SerializeField] public int MoveTextSize { get; private set; } = 300;
    public bool ChatBoxOpen { get; private set; }

    //Local Fields
    TextPopup _currentTextPopup;
    Queue<string> RemainingTexts = new();

    [Space(10)]
    [Header("Current State")]
    bool allTextWritten = true;
    float timer;
    int characterIndex;
    string CurrentStringToWrite;

    [Space(10)]
    [Header("Componenets")]
    [SerializeField] CanvasGroup canvas;
    [SerializeField] Image popupImage;
    [SerializeField] TextMeshProUGUI nameTitleText;
    [SerializeField] TextMeshProUGUI _uIText;

    //Helpers
    bool canSkipText { get { return _currentTextPopup.Skippable && !allTextWritten; } }
    string Text { get { return _currentTextPopup.Text; } }
    float TimePerCharacter { get { return _currentTextPopup.TimePerCharacter; } }
    bool canCloseChatBox { get { return allTextWritten && RemainingTexts.Count <= 0; } }

    private void Update()
    {
        if (allTextWritten)
            return;

        WriteText();

        void WriteText()
        {
            if (CurrentStringToWrite == "") { allTextWritten = true; return; }

            //Set timer to frame time
            timer -= Time.deltaTime;

            //Add visible characters to the textbox for this frame
            while (timer <= 0f)
            {
                characterIndex++;
                timer += TimePerCharacter;

                //Place current written text
                string text = CurrentStringToWrite.Substring(0, characterIndex);
                //Place rest of text as invisible
                text += "<color=#00000000>" + CurrentStringToWrite.Substring(characterIndex) + "</color>";

                _uIText.text = text;

                //If we reach the end of the text then stop
                if (characterIndex >= CurrentStringToWrite.Length)
                {
                    allTextWritten = true;
                    return;
                }
            }
        }
    }

    public bool StartNewPopup(TextPopup popup)
    {
        if (ChatBoxOpen)
            return false;
        _currentTextPopup = popup;
        InitializeNewPopup();
        return true;
    }

    void CloseChatBox()
    {
        OnChatBoxClose?.Invoke();
        ChatBoxOpen = false;
        canvas.SetCanvas(false);
    }

    public void OnClickBackground()
    {

        //Text is still writing so try and skip it
        if (canSkipText)
        {
            WriteAllText();
            return;
        }

        //Text is done writing and there are no more to write
        if (canCloseChatBox)
        {
            CloseChatBox();
            return;
        }

        //Text is done writing and there are more to write
        if (allTextWritten)
        {
            StartNextText();
        }

        void WriteAllText()
        {
            _uIText.text = CurrentStringToWrite;
            allTextWritten = true;
        }

    }

    void StartNextText()
    {
        resetWriter();
        CurrentStringToWrite = RemainingTexts.Dequeue();

        void resetWriter()
        {
            _uIText.text = "";
            allTextWritten = false;
            characterIndex = 0;
            timer = 0;
            _uIText.margin = Vector4.zero;
        }
    }

    public void InitializeNewPopup()
    {
        ChatBoxOpen = true;
        canvas.SetCanvas(true);
        SplitUpTextToFitUI();
        StartNextText();

        void SplitUpTextToFitUI()
        {
            RemainingTexts.Clear();
            int CurrentCharacterIndex = 0;
            while (Text.Length - CurrentCharacterIndex > MaxCharPerPopup)
            {
                StringBuilder sb = new();
                sb.Append(Text.Substring(CurrentCharacterIndex, MaxCharPerPopup - 1));
                while (sb[sb.Length - 1] != ' ' && sb.Length <= Text.Length - 1)
                {
                    sb.Append(Text[sb.Length + CurrentCharacterIndex]);
                }
                RemainingTexts.Enqueue(sb.ToString());
                CurrentCharacterIndex += sb.Length;
            }

            RemainingTexts.Enqueue(Text.Substring(CurrentCharacterIndex));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickBackground();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}

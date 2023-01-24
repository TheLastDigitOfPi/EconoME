using UnityEngine;
using System;
using System.Collections;
/// <summary>
/// Initializes Chat box handler with current environment, as well as creates and destroys it as needed
/// </summary>
public class ChatBoxManager : MonoBehaviour
{
    //Statics
    public static ChatBoxManager Instance;

    //Public fields
    public bool ChatBoxActive { get { return ChatBoxBusy; } }

    //Local fields
    [SerializeField] ChatBoxHandler _chatBoxPrefab;
    ChatBoxHandler _activeChatBox;

    //Events
    public event Action OnTextPopupStart;
    public event Action OnTextPopupEnd;

    //Helper fields
    bool ChatBoxBusy { get { return _activeChatBox != null ? _activeChatBox.ChatBoxOpen : false; } }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than 1 chat box manager found!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public bool PlayText(TextPopup popup)
    {
        if (ChatBoxBusy)
            return false;
        StopAllCoroutines();
        OnTextPopupStart?.Invoke();
        if (_activeChatBox == null)
        {
            _activeChatBox = Instantiate(_chatBoxPrefab, transform);
        }

        _activeChatBox.OnChatBoxClose += OnEndText;
        return _activeChatBox.StartNewPopup(popup);
    }

    /// <summary>
    /// Will Destroy the chat box if it is not being used after close for x seconds
    /// </summary>
    void OnEndText()
    {
        OnTextPopupEnd?.Invoke();
        _activeChatBox.OnChatBoxClose -= OnEndText;
        StartCoroutine(DestroyAfterNoActiveChat());
        IEnumerator DestroyAfterNoActiveChat()
        {
            yield return new WaitForSeconds(10f);
            Destroy(_activeChatBox.gameObject);
            _activeChatBox = null;
        }
    }

}

/*
 
    Interactions should be queued up in groups

    Textpopup -> recieve item -> textpopup -> textpopup with choice -> ? How do we split the textpopup choices? 

    Add popups for each choice and go to that popup on the choice?
    Textpopups with choices will add a new interaction set to the queue - yes

    I will need to design a single interaction set that takes in interactions and can reject invalid interactions (I cannot put a text popup directly after a choice as the choice will determine what the next popup is and continue the chain.
 
 
 
 */
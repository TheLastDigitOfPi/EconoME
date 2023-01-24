using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Text Popup", menuName = "ScriptableObjects/Interactions/Interactions/Text Popup")]
public class TextPopup : Interaction
{
    [Multiline] public string Text;
    [SerializeField] Sprite _icon;
    public Sprite Icon { get { return _icon; } }
    public float TimePerCharacter = 0.02f;
    public string titleName;

    [field: SerializeField] public bool Skippable { get; private set; }

    public Interaction[] onClose;

    public override event Action OnInteractionEnd;

    public override void Activate(InteractionHandler handler)
    {
        if (ChatBoxManager.Instance.PlayText(this))
        {
            ChatBoxManager.Instance.OnTextPopupEnd += EndInteraction;
        }
    }

    void EndInteraction()
    {
        OnInteractionEnd?.Invoke();
        ChatBoxManager.Instance.OnTextPopupEnd -= EndInteraction;
    }


}


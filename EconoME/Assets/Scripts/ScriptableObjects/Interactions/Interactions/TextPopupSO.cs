using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Text Popup", menuName = "ScriptableObjects/Interactions/Interactions/Text Popup")]
public class TextPopupSO : InteractionSO
{
    [Multiline] public string Text;
    [SerializeField] Sprite _icon;
    public Sprite Icon { get { return _icon; } }
    public float TimePerCharacter = 0.02f;
    public string titleName;
    
    public InteractionSO[] onClose;
    [field: SerializeField] public bool Skippable { get; private set; }
    public override Interaction GetInteraction()
    {
        return new TextPopup(ID, TimePerCharacter, titleName, Icon, Text, Skippable, onClose);
    }
}

public class TextPopup : Interaction
{
    [Multiline] public string Text;
    [SerializeField] Sprite _icon;
    public Sprite Icon { get { return _icon; } }
    public float TimePerCharacter = 0.02f;
    public string titleName;

    [field: SerializeField] public bool Skippable { get; private set; }

    public InteractionSO[] onClose;

    public TextPopup(Guid interactionId, float timePercharacter, string titleName, Sprite Icon, string Text, bool skippable, InteractionSO[] onClose) : base(interactionId)
    {
        this.TimePerCharacter = timePercharacter;
        this.titleName = titleName;
        this.Text = Text;
        this._icon= Icon;
        this.Skippable = skippable;
        this.onClose = onClose;
    }

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


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

    public Interaction[] onClose;

    public override void Activate(InteractionHandler handler)
    {

        handler.DisableOldUI();
        SplitUpTextToFitUI(handler);
        SetHandlerToThisPopup(handler);
        void SplitUpTextToFitUI(InteractionHandler handler)
        {
            int CurrentCharacterIndex = 0;
            while (Text.Length - CurrentCharacterIndex > handler.MaxCharPerPopup)
            {
                StringBuilder sb = new();
                sb.Append(Text.Substring(CurrentCharacterIndex, handler.MaxCharPerPopup - 1));
                while (sb[sb.Length - 1] != ' ' && sb.Length <= Text.Length - 1)
                {
                    sb.Append(Text[sb.Length + CurrentCharacterIndex]);
                }
                handler.RemainingTexts.Add(sb.ToString());
                CurrentCharacterIndex += sb.Length;
            }

            handler.RemainingTexts.Add(Text.Substring(CurrentCharacterIndex));
        }
        void SetHandlerToThisPopup(InteractionHandler handler)
        {
            handler.CurrentStringToWrite = handler.RemainingTexts[0];
            handler.nameTitleText.text = titleName;
            handler.RemainingTexts.RemoveAt(0);
            if (handler.RemainingTexts.Count == 0) { handler.onFinalText?.Invoke(); handler.onFinalText = null; }
            handler.CurrentTimePerCharacter = TimePerCharacter;
        }

    }


}


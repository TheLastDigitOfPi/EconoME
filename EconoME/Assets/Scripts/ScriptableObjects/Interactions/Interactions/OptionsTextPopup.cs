using System.Text;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Options Text Popup", menuName = "ScriptableObjects/Interactions/Interactions/Options Text Popup")]
public class OptionsTextPopup : TextPopup
{
    public Option[] Options { get { return _options; } }
    [SerializeField] Option[] _options;

    public override void Activate(InteractionHandler handler)
    {
        handler.DisableOldUI();

        SplitUpTextToFitUI(handler);

        SetOptionsToShowUpOnFinalText(handler);

        SetHandlerToThisPopup(handler);

        void SetOptionsToShowUpOnFinalText(InteractionHandler handler)
        {
            handler.onFinalText += () =>
            {
                handler.UIText.margin = new Vector4(0, 0, handler.MoveTextSize, 0);
                handler.canCancelText = false;

                for (int i = 0; i < 9; i++)
                {
                    if (i <= Options.Length - 1)
                    {
                        handler.OptionsButtons[i].gameObject.SetActive(true);
                        handler.OptionsButtons[i].onClick.RemoveAllListeners();
                        handler.OptionsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = Options[i].optionText;
                        if (Options[i].onSelect != null)
                        { handler.OptionsButtons[i].onClick.AddListener(Options[i].onSelect); }
                        handler.OptionsButtons[i].onClick.AddListener(() => { handler.nextText = true; handler.allTextWritten = false; });
                    }
                    else
                    {
                        handler.OptionsButtons[i].gameObject.SetActive(false);
                    }
                }


            };
        }

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
            handler.RemainingTexts.RemoveAt(0);
            if (handler.RemainingTexts.Count == 0) { handler.onFinalText?.Invoke(); handler.onFinalText = null; }
            handler.CurrentTimePerCharacter = TimePerCharacter;
        }
    }
}


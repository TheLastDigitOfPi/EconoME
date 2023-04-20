using System.Text;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Options Text Popup", menuName = "ScriptableObjects/Interactions/Interactions/Options Text Popup")]
public class OptionsTextPopup : TextPopupSO
{
    public Option[] Options { get { return _options; } }
    [SerializeField] Option[] _options;
    /*
    public override void Activate(InteractionHandler handler)
    {
        base.Activate(handler);

        SetOptionsToShowUpOnFinalText(handler);
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
       
    }
    */
}


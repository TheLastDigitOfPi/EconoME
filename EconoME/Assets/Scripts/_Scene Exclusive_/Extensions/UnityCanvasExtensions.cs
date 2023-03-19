using UnityEngine;

public static class UnityCanvasExtensions
{
    public static void ToggleCanvas(this CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = canvasGroup.alpha == 1 ? 0 : 1;
        canvasGroup.interactable = !canvasGroup.interactable;
        canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
    }

    public static void SetCanvas(this CanvasGroup canvasGroup, bool CanvasEnabled)
    {
        canvasGroup.alpha = CanvasEnabled ? 1 : 0;
        canvasGroup.interactable = CanvasEnabled;
        canvasGroup.blocksRaycasts = CanvasEnabled;
    }
}


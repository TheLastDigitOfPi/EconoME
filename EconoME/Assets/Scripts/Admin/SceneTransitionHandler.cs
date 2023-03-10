using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionHandler : MonoBehaviour
{
    //public events
    public event Action OnScreenInvisible;
    public event Action OnScreenVisible;

    //settings
    [field: SerializeField] public float AnimationTime { get; private set; } = 1f;
    //required
    [SerializeField] CanvasGroup ProgressCanvas;
    [SerializeField] Animator anim;
    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI progressText;


    bool screenInvisible = false;

    public void Initialize()
    {
        GlobalSceneManager.OnSceneLoad += StartScreenToVisible;
        GlobalSceneManager.OnSceneStartDisable += StartScreenToBlack;
        ProgressCanvas.SetCanvas(false);
    }

    public void StartScreenToVisible()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOut());
        IEnumerator FadeOut()
        {
            yield return new WaitUntil(() => screenInvisible);
            ProgressCanvas.SetCanvas(false);
            anim.CrossFade("SquareCoverIn", 0);
            yield return new WaitForSeconds(0.9f);
            OnScreenVisible?.Invoke();

        }
    }
    public void StartScreenToBlack()
    {
        StartCoroutine(FadeIn());
        IEnumerator FadeIn()
        {
            ProgressCanvas.SetCanvas(false);
            screenInvisible = false;
            anim.CrossFade("SquareCoverOut", 0);
            yield return new WaitForSeconds(0.9f);
            OnScreenInvisible?.Invoke();
            screenInvisible = true;
            //yield return new WaitForSeconds(0.5f);
            ProgressCanvas.SetCanvas(true);

        }
    }

    private void OnDestroy()
    {
        GlobalSceneManager.OnSceneLoad -= StartScreenToVisible;
        GlobalSceneManager.OnSceneStartDisable -= StartScreenToBlack;
    }

    public void UpdateStatus(AsyncOperation loadOperation, AsyncOperation unloadOperation)
    {
        float loadProgress = Mathf.Clamp01(loadOperation.progress / 0.9f);
        float unloadProgess = Mathf.Clamp01(unloadOperation.progress / 0.9f);

        float totalProgess = (loadProgress + unloadProgess) / 2;
        slider.value = totalProgess;
        progressText.text = $"{(int)(totalProgess * 100)}%";
        Debug.Log(totalProgess);
    }
}


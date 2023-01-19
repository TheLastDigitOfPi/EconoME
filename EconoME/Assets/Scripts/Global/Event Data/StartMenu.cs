using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{

    [SerializeField] bool useStartTransition;
    [SerializeField] Animator anim;
    [SerializeField] float AnimationTime = 1f;
    [SerializeField] CanvasGroup startScreenCanvas;
    [SerializeField] CanvasGroup ProgressCanvas;


    [SerializeField] Slider slider;
    [SerializeField] TextMeshProUGUI progressText;

    public void LoadGame(int SceneID)
    {
        StartCoroutine(LoadScene(SceneID));
    }

    public void NewGame(int SceneID)
    {
        StartCoroutine(LoadScene(SceneID));
    }

    public void Settings()
    {

    }

    private void Start()
    {
        if (useStartTransition)
        {
            StartCoroutine(StartGame());
        }
    }

    IEnumerator StartGame()
    {
        anim.CrossFade("SquareCoverIn", 0);
        yield return new WaitForSeconds(0.9f);
        GameLoopEvents.gameStarted?.Invoke();
    }

    IEnumerator LoadScene(int sceneIndex)
    {
        startScreenCanvas.interactable = false;
        startScreenCanvas.blocksRaycasts = false;
        anim.CrossFade("SquareCoverOut", 0);
        yield return new WaitForSeconds(AnimationTime / 2);

        ProgressCanvas.alpha = 1;
        yield return new WaitForSeconds(AnimationTime / 2);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value = progress;
            progressText.text = $"{(int)(progress * 100)}%";
            Debug.Log(progress);
            yield return null;
        }
    }
}

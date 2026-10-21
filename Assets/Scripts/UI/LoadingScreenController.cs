using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    public static LoadingScreenController Instance { get; private set; }
    public static bool IsTransitioning { get; private set; } = false;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float minShowTime = 0.4f;
    [SerializeField] private float dotInterval = 0.4f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void RunTransition(Action action)
    {
        StartCoroutine(TransitionRoutine(action));
    }

    public void LoadSceneWithTransition(string sceneName, bool waitForGameLoaded = false)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, waitForGameLoaded));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, bool waitForGameLoaded)
    {
        IsTransitioning = true;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        Coroutine dots = StartCoroutine(AnimateDots());

        float startTime = Time.unscaledTime;

        bool gameLoadedReceived = false;
        void Handler() => gameLoadedReceived = true;

        if (waitForGameLoaded)
        {
            SaveController.OnGameLoaded += Handler;
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }

        if (waitForGameLoaded)
        {
            while (!gameLoadedReceived)
            {
                yield return null;
            }
            SaveController.OnGameLoaded -= Handler;
        }

        float elapsed = Time.unscaledTime - startTime;
        if (elapsed < minShowTime)
        {
            yield return new WaitForSecondsRealtime(minShowTime - elapsed);
        }

        yield return FadeOut();

        StopCoroutine(dots);
        canvasGroup.blocksRaycasts = false;
        IsTransitioning = false;
    }

    private IEnumerator TransitionRoutine(Action action)
    {
        IsTransitioning = true;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Coroutine dots = StartCoroutine(AnimateDots());

        action?.Invoke();

        Physics2D.SyncTransforms();

        yield return new WaitForSecondsRealtime(minShowTime);

        yield return FadeOut();

        StopCoroutine(dots);
        canvasGroup.blocksRaycasts = false;

        IsTransitioning = false;
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private IEnumerator AnimateDots()
    {
        string[] frames = { "Carregando", "Carregando.", "Carregando..", "Carregando..." };
        int i = 0;
        while (true)
        {
            loadingText.text = frames[i % frames.Length];
            i++;
            yield return new WaitForSecondsRealtime(dotInterval);
        }
    }
}
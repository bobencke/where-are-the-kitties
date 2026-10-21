using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class InitialCutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private string gameLevel = "Jogo";
    [SerializeField] private NPCDialogue introSpeechDialogue;

    void OnEnable()
    {
        director.stopped += OnCutsceneFinished;
    }

    void OnDisable()
    {
        director.stopped -= OnCutsceneFinished;
    }

    private void OnCutsceneFinished(PlayableDirector pd)
    {
        PlayerPrefs.SetInt("InitialCutscene", 1);
        PlayerSpeechController.PendingIntroDialogue = introSpeechDialogue;

        if (LoadingScreenController.Instance != null)
        {
            LoadingScreenController.Instance.LoadSceneWithTransition(gameLevel, waitForGameLoaded: true);
        }
        else
        {
            SceneManager.LoadScene(gameLevel);
        }
    }
}
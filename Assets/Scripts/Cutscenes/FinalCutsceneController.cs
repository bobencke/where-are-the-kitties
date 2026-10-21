using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class FinalCutsceneController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private double loopStartTime = 0.0;
    [SerializeField] private int loopCount = 10;
    [SerializeField] private string menuScene = "Menu";

    private int currentLoop = 0;

    void OnEnable()
    {
        director.stopped += OnCutsceneFinished;
    }

    void OnDisable()
    {
        director.stopped -= OnCutsceneFinished;
    }

    public void HandleLoopPoint()
    {
        currentLoop++;

        if (currentLoop < loopCount)
        {
            director.time = loopStartTime;
            director.Play();
        }
    }

    private void OnCutsceneFinished(PlayableDirector pd)
    {
        ResetGame();

        if (LoadingScreenController.Instance != null)
        {
            LoadingScreenController.Instance.LoadSceneWithTransition(menuScene);
        }
        else
        {
            SceneManager.LoadScene(menuScene);
        }
    }

    private void ResetGame()
    {
        string saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        if (File.Exists(saveLocation))
        {
            File.Delete(saveLocation);
        }

        PlayerPrefs.DeleteKey("InitialCutscene");
        PlayerPrefs.Save();
    }
}
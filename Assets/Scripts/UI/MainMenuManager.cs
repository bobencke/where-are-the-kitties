using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameLevel;
    [SerializeField] private string initialCutsceneLevel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    public void Play()
    {
        bool jaViuCutscene = PlayerPrefs.GetInt("InitialCutscene", 0) == 1;
        string targetScene = jaViuCutscene ? gameLevel : initialCutsceneLevel;

        if (LoadingScreenController.Instance != null)
        {
            LoadingScreenController.Instance.LoadSceneWithTransition(targetScene, waitForGameLoaded: jaViuCutscene);
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }
    }

    public void OpenOptions()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo");
        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    [SerializeField] private string mainMenuScene = "Menu";

    void Start()
    {
        menuCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;
            }
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            PauseController.SetPause(menuCanvas.activeSelf);
        }
    }

    public void ReturnToMainMenu()
    {
        PauseController.SetPause(false);

        if (LoadingScreenController.Instance != null)
        {
            LoadingScreenController.Instance.LoadSceneWithTransition(mainMenuScene);
        }
        else
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}
using UnityEngine;

public class Letter : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject letterScreen;
    [SerializeField] private NPCDialogue afterReadingDialogue;

    public bool CanInteract()
    {
        return true;
    }

    public void Interact()
    {
        letterScreen.SetActive(true);
        PauseController.SetPause(true);
    }

    public void CloseLetter()
    {
        letterScreen.SetActive(false);

        if (afterReadingDialogue != null && PlayerSpeechController.Instance != null)
        {
            PlayerSpeechController.Instance.ShowDialogue(afterReadingDialogue);
        }
        else
        {
            PauseController.SetPause(false);
        }
    }

    void Update()
    {
        if (letterScreen.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseLetter();
        }
    }
}
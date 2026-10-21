using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpeechController : MonoBehaviour
{
    public static PlayerSpeechController Instance { get; private set; }
    public static NPCDialogue PendingIntroDialogue;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image portraitImage;

    private NPCDialogue currentDialogue;
    private int dialogueIndex;
    private bool isTyping;

    public bool IsSpeaking { get; private set; }

    void Awake()
    {
        Instance = this;
        SaveController.OnGameLoaded += HandleGameLoaded;
    }

    void OnDestroy()
    {
        SaveController.OnGameLoaded -= HandleGameLoaded;
    }

    private void HandleGameLoaded()
    {
        if (PendingIntroDialogue != null)
        {
            NPCDialogue dialogue = PendingIntroDialogue;
            PendingIntroDialogue = null;
            ShowDialogue(dialogue);
        }
    }

    public void ShowDialogue(NPCDialogue dialogue)
    {
        if (dialogue == null || IsSpeaking) return;

        currentDialogue = dialogue;
        dialogueIndex = 0;
        IsSpeaking = true;

        if (nameText != null) nameText.SetText(currentDialogue.npcName);
        if (portraitImage != null) portraitImage.sprite = currentDialogue.npcPortrait;

        dialoguePanel.SetActive(true);
        PauseController.SetPause(true);

        StartCoroutine(TypeLine());
    }

    public void AdvanceOrClose()
    {
        if (!IsSpeaking) return;

        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.SetText(currentDialogue.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < currentDialogue.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndSpeech();
        }
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach (char letter in currentDialogue.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            SoundEffectManager.PlayVoice(currentDialogue.voiceSound, currentDialogue.voicePitch);
            yield return new WaitForSeconds(currentDialogue.typingSpeed);
        }

        isTyping = false;

        if (currentDialogue.autoProgressLines.Length > dialogueIndex && currentDialogue.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(currentDialogue.autoProgressDelay);
            AdvanceOrClose();
        }
    }

    public void OnCloseButtonPressed()
    {
        if (IsSpeaking)
        {
            EndSpeech();
        }
        else
        {
            NPC.currentNPC?.CloseDialogue();
        }
    }

    private void EndSpeech()
    {
        StopAllCoroutines();
        IsSpeaking = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        PauseController.SetPause(false);
    }
}
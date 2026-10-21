using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    protected Animator animator;

    public static NPC currentNPC;

    public NPCDialogue dialogueData;
    public NPCDialogue normalRejectionDialogue;
    public NPCDialogue fishRejectionDialogue;
    public NPCDialogue moneyRejectionDialogue;
    public NPCDialogue pescatoMoneyRejectionDialogue;
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Image portraitImage;

    public string NpcID { get; private set; }
    public bool IsQuestCompleted { get; protected set; }

    protected NPCDialogue currentDialogue;
    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private bool interactionDisabled = false;

    protected virtual void Awake()
    {
        NpcID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    public bool CanInteract()
    {
        return !isDialogueActive && !interactionDisabled;
    }

    protected void DisableInteraction()
    {
        interactionDisabled = true;
    }

    protected void MarkQuestCompleted()
    {
        IsQuestCompleted = true;
    }

    public void Interact()
    {
        if (PauseController.IsGamePaused && !isDialogueActive)
        {
            return;
        }

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            NPCDialogue nextDialogue = GetDialogueToUse();
            ShowDialogue(nextDialogue);
        }
    }

    protected virtual NPCDialogue GetDialogueToUse()
    {
        return dialogueData;
    }

    public void ShowDialogue(NPCDialogue dialogue)
    {
        if (dialogue == null || isDialogueActive)
        {
            return;
        }

        currentDialogue = dialogue;
        StartDialogue();
    }

    public void ShowDynamicDialogue(string[] lines)
    {
        NPCDialogue baseDialogue = GetDialogueToUse();

        NPCDialogue dynamicDialogue = ScriptableObject.CreateInstance<NPCDialogue>();
        dynamicDialogue.npcName = baseDialogue != null ? baseDialogue.npcName : "";
        dynamicDialogue.npcPortrait = baseDialogue != null ? baseDialogue.npcPortrait : null;
        dynamicDialogue.voiceSound = dialogueData != null ? dialogueData.voiceSound : null;
        dynamicDialogue.voicePitch = dialogueData != null ? dialogueData.voicePitch : 1.0f;
        dynamicDialogue.dialogueLines = lines;
        dynamicDialogue.autoProgressLines = new bool[lines.Length];
        dynamicDialogue.autoProgressDelay = 1.5f;
        dynamicDialogue.typingSpeed = 0.05f;

        ShowDialogue(dynamicDialogue);
    }

    void StartDialogue()
    {
        NPC.currentNPC = this;

        if (animator != null)
        {
            animator.SetBool("isTalking", true);
        }

        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.SetText(currentDialogue.npcName);
        portraitImage.sprite = currentDialogue.npcPortrait;

        dialoguePanel.SetActive(true);
        PauseController.SetPause(true);

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
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
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
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
            NextLine();
        }
    }

    protected virtual void EndDialogue()
    {
        StopAllCoroutines();

        if (animator != null)
        {
            animator.SetBool("isTalking", false);
        }

        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);
        PauseController.SetPause(false);

        if (currentNPC == this)
        {
            currentNPC = null;
        }
    }

    public void OnCloseButtonPressed()
    {
        NPC.currentNPC?.EndDialogue();
    }

    public void CloseDialogue()
    {
        EndDialogue();
    }

    public virtual List<NpcFlag> GetSaveFlags()
    {
        List<NpcFlag> flags = new List<NpcFlag>();

        FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(bool))
            {
                flags.Add(new NpcFlag { key = field.Name, value = (bool)field.GetValue(this) });
            }
        }

        flags.Add(new NpcFlag { key = "IsQuestCompleted", value = IsQuestCompleted });

        return flags;
    }

    public virtual void ApplySaveFlags(List<NpcFlag> flags)
    {
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (NpcFlag flag in flags)
        {
            if (flag.key == "IsQuestCompleted")
            {
                IsQuestCompleted = flag.value;
                continue;
            }

            FieldInfo field = fields.FirstOrDefault(f => f.Name == flag.key && f.FieldType == typeof(bool));
            if (field != null)
            {
                field.SetValue(this, flag.value);
            }
        }

        OnFlagsRestored();
    }

    protected virtual void OnFlagsRestored() { }

    public Vector3 GetSavePosition()
    {
        return transform.position;
    }

    public void ApplySavePosition(Vector3 position)
    {
        transform.position = position;

        NPCFollower follower = GetComponent<NPCFollower>();
        if (follower != null)
        {
            follower.WarpTo(position);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class FlorController : NPC
{
    private PescatoController pescato;

    public bool firstInteractionHapened = false;

    public List<NPCDialogue> florDialogues;

    protected override void Start()
    {
        base.Start();
        pescato = FindFirstObjectByType<PescatoController>();
    }

    protected override void OnFlagsRestored()
    {
        base.OnFlagsRestored();

        if (IsQuestCompleted)
        {
            DisableInteraction();

            if (animator != null)
            {
                animator.SetBool("isFollowing", true);
            }

            GetComponent<NPCFollower>()?.StartFollowing();
        }
    }

    protected override NPCDialogue GetDialogueToUse()
    {
        if (pescato.hasGivenReward)
        {
            return florDialogues[1];
        }

        return florDialogues[0];
    }

    protected override void EndDialogue()
    {
        base.EndDialogue();

        if (currentDialogue == florDialogues[0])
        {
            firstInteractionHapened = true;
        }
        else if (currentDialogue == florDialogues[1])
        {
            MarkQuestCompleted();

            animator.SetBool("isFollowing", true);
            GetComponent<NPCFollower>().StartFollowing();
            DisableInteraction();
        }
    }
}
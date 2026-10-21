using System.Collections.Generic;
using UnityEngine;

public class FionaController : NPC
{
    public bool hasCompletedQuest = false;

    public List<NPCDialogue> fionaDialogues;
    public GameObject oCordaoPrefab;

    protected override void OnFlagsRestored()
    {
        base.OnFlagsRestored();

        if (hasCompletedQuest)
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
        return fionaDialogues[0];
    }

    public bool TryExchangeOCordao(NPC npc)
    {
        int oCordaoID = oCordaoPrefab.GetComponent<Item>().ID;

        if (!HotBarController.Instance.HasItem(oCordaoID, 1))
        {
            return false;
        }

        HotBarController.Instance.RemoveItem(oCordaoID, 1);

        npc.ShowDialogue(fionaDialogues[1]);

        hasCompletedQuest = true;
        MarkQuestCompleted();

        return true;
    }

    protected override void EndDialogue()
    {
        base.EndDialogue();


        if (currentDialogue == fionaDialogues[1])
        {
            animator.SetBool("isFollowing", true);
            GetComponent<NPCFollower>().StartFollowing();
            DisableInteraction();
        }
    }
}
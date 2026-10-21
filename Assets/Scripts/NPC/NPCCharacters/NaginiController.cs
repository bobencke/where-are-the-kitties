using System.Collections.Generic;
using UnityEngine;

public class NaginiController : NPC
{
    public List<NPCDialogue> naginiDialogues;
    public GameObject strawberryPrefab;

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
        return naginiDialogues[0];
    }

    protected override void EndDialogue()
    {
        base.EndDialogue();

        if (currentDialogue == naginiDialogues[1])
        {
            animator.SetBool("isFollowing", true);
            GetComponent<NPCFollower>().StartFollowing();
            DisableInteraction();
        }
    }

    public bool TryGiveStrawberry(NPC npc)
    {
        int strawberryID = strawberryPrefab.GetComponent<Item>().ID;

        if (!HotBarController.Instance.HasItem(strawberryID, 1))
        {
            return false;
        }

        HotBarController.Instance.RemoveItem(strawberryID, 1);

        npc.ShowDialogue(naginiDialogues[1]);

        MarkQuestCompleted();

        return true;
    }

}
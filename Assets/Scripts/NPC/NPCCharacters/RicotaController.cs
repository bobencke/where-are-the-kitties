using System.Collections.Generic;
using UnityEngine;

public class RicotaController : NPC
{
    public bool hasCompletedQuest = false;

    public List<NPCDialogue> ricotaDialogues;
    public GameObject pastaPrefab;
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
        return ricotaDialogues[0];
    }

    public bool TryExchangePasta(NPC npc)
    {
        int pastaID = pastaPrefab.GetComponent<Item>().ID;

        if (!HotBarController.Instance.HasItem(pastaID, 1))
        {
            return false;
        }

        HotBarController.Instance.RemoveItem(pastaID, 1);

        npc.ShowDialogue(ricotaDialogues[1]);

        hasCompletedQuest = true;
        MarkQuestCompleted();

        return true;
    }

    public bool CheckPlayerInventory()
    {
        int requiredItemID = pastaPrefab.GetComponent<Item>().ID;
        return HotBarController.Instance.HasItem(requiredItemID, 1);
    }

    protected override void EndDialogue()
    {
        base.EndDialogue();


        if (currentDialogue == ricotaDialogues[1])
        {
            DropOCordao();
            animator.SetBool("isFollowing", true);
            GetComponent<NPCFollower>().StartFollowing();
            DisableInteraction();
        }
    }

    private void DropOCordao()
    {
        GameObject droppedItem = Instantiate(oCordaoPrefab, transform.position + Vector3.down * 2, Quaternion.identity);

        Item item = droppedItem.GetComponent<Item>();

        item.quantity = 1;
        item.UpdateQuantityDisplay();

        BounceEffect bounce = droppedItem.GetComponent<BounceEffect>();
        
        if (bounce != null)
        {
            bounce.StartBounce();
        }
    }
}

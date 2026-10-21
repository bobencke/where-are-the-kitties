using System.Collections.Generic;
using UnityEngine;

public class RobertoController : NPC
{
    private FionaController fiona;

    public bool hasGivenReward = false;

    public List<NPCDialogue> robertoDialogues;
    public GameObject rewardPrefab;

    protected override void Start()
    {
        base.Start();
        fiona = FindFirstObjectByType<FionaController>();
    }

    protected override NPCDialogue GetDialogueToUse()
    {
        if (hasGivenReward)
        {
            return robertoDialogues[2];
        }

        if (fiona.hasCompletedQuest)
        {
            return robertoDialogues[1];
        }

        return robertoDialogues[0];
    }

    protected override void EndDialogue()
    {
        base.EndDialogue();

        if (currentDialogue == robertoDialogues[1] && !hasGivenReward)
        {
            DropReward();
            hasGivenReward = true;
        }
    }

    private void DropReward()
    {
        GameObject droppedItem = Instantiate(rewardPrefab, transform.position + Vector3.down * 2, Quaternion.identity);

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
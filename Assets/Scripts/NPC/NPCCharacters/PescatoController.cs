using System.Collections.Generic;
using UnityEngine;

public class PescatoController : NPC
{
    private FlorController flor;

    public bool hasIntroducedHimself = false;
    public bool hasAcceptedQuest = false;
    public bool hasGivenReward = false;

    public List<NPCDialogue> pescatoDialogues;
    public GameObject fishPrefab;
    public GameObject pescatoMoneyPrefab;
    public GameObject rewardPrefab;

    protected override void Start()
    {
        base.Start();
        flor = FindFirstObjectByType<FlorController>();
    }

    protected override NPCDialogue GetDialogueToUse()
    {
        if (!hasIntroducedHimself)
        {
            return pescatoDialogues[0];
        }
        else if (!flor.firstInteractionHapened)
        {
            return pescatoDialogues[0];
        }
        else if (flor.firstInteractionHapened && !hasAcceptedQuest)
        {
            return pescatoDialogues[1];
        }
        else if (hasAcceptedQuest && !hasGivenReward)
        {
            return pescatoDialogues[2];
        }

        return pescatoDialogues[4];
    }

    protected override void EndDialogue()
    {
        base.EndDialogue();

        if (currentDialogue == pescatoDialogues[0])
        {
            hasIntroducedHimself = true;
        }
        else if (currentDialogue == pescatoDialogues[1])
        {
            hasAcceptedQuest = true;
            DropFish();
        }
        else if(currentDialogue == pescatoDialogues[3])
        {
            DropReward();
        }
    }

    public bool TryExchangeMoney(NPC npc)
    {
        if (hasGivenReward)
        {
            return false;
        }

        if (!CheckPlayerInventory())
        {
            return false;
        }

        int requiredItemID = pescatoMoneyPrefab.GetComponent<Item>().ID;
        HotBarController.Instance.RemoveItem(requiredItemID, 3);

        npc.ShowDialogue(pescatoDialogues[3]);

        hasGivenReward = true;
        return true;
    }

    public bool CheckPlayerInventory()
    {
        int requiredItemID = pescatoMoneyPrefab.GetComponent<Item>().ID;
        return HotBarController.Instance.HasItem(requiredItemID, 3);
    }

    private void DropFish()
    {
        GameObject droppedItem = Instantiate(fishPrefab, transform.position + Vector3.down * 2, Quaternion.identity);
        Item item = droppedItem.GetComponent<Item>();
        item.quantity = 3;
        item.UpdateQuantityDisplay();
        droppedItem.GetComponent<BounceEffect>().StartBounce();
    }

    private void DropReward()
    {
        GameObject droppedItem = Instantiate(rewardPrefab, transform.position + Vector3.down * 2, Quaternion.identity);
        Item item = droppedItem.GetComponent<Item>();
        item.quantity = 1;
        item.UpdateQuantityDisplay();
        droppedItem.GetComponent<BounceEffect>().StartBounce();
    }
}
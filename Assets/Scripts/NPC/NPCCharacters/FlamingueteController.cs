using System.Collections.Generic;
using UnityEngine;

public class FlamingueteController : NPC
{
    public bool hasGivenPasta = false;

    public GameObject moneyPrefab;
    public GameObject pastaPrefab;

    public List<NPCDialogue> flamingueteDialogues;

    protected override NPCDialogue GetDialogueToUse()
    {
        return flamingueteDialogues[0];
    }
    protected override void EndDialogue()
    {
        base.EndDialogue();

        if (currentDialogue == flamingueteDialogues[1])
        {
            DropPasta();
        }
    }

    public bool TryGivePasta(NPC npc)
    {
        if (hasGivenPasta)
        {
            return false;
        }

        int moneyID = moneyPrefab.GetComponent<Item>().ID;

        if (!HotBarController.Instance.HasItem(moneyID, 1))
        {
            return false;
        }

        HotBarController.Instance.RemoveItem(moneyID, 1);

        npc.ShowDialogue(flamingueteDialogues[1]);

        hasGivenPasta = true;

        return true;
    }

    private void DropPasta()
    {
        GameObject droppedItem = Instantiate(pastaPrefab, transform.position + Vector3.down * 2, Quaternion.identity);
        Item item = droppedItem.GetComponent<Item>();
        item.quantity = 1;
        item.UpdateQuantityDisplay();
        droppedItem.GetComponent<BounceEffect>().StartBounce();
    }
}
using System.Collections.Generic;
using UnityEngine;

public class GiMorangoController : NPC
{
    public List<NPCDialogue> giDialogues;
    public GameObject strawberryMoneyPrefab;
    public GameObject strawberryPrefab;

    protected override NPCDialogue GetDialogueToUse()
    {
        return giDialogues[0];
    }

    protected override void EndDialogue()
    {
        base.EndDialogue();

        if (currentDialogue == giDialogues[1])
        {
            DropStrawberrry();
        }
    }

    public bool TryGetStrawberry(NPC npc)
    {
        int strawberryMoneyID = strawberryMoneyPrefab.GetComponent<Item>().ID;

        if (!HotBarController.Instance.HasItem(strawberryMoneyID, 1))
        {
            return false;
        }

        HotBarController.Instance.RemoveItem(strawberryMoneyID, 1);

        npc.ShowDialogue(giDialogues[1]);

        return true;
    }

    private void DropStrawberrry()
    {
        GameObject droppedItem = Instantiate(strawberryPrefab, transform.position + Vector3.down * 2, Quaternion.identity);

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

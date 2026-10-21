using UnityEngine;

public class ItemBuyer : MonoBehaviour
{
    public int acceptedItemID;
    public NPCDialogue thankYouDialogue;
    public NPCDialogue rejectionDialogue;
    public GameObject rewardPrefab;
    public int rewardQuantity = 1;
    public bool hasBuyedItem = false;

    public bool CanBuy(int itemID)
    {
        return itemID == acceptedItemID;
    }

    public bool TrySell(NPC npc)
    {
        if (hasBuyedItem)
        {
            npc.ShowDialogue(rejectionDialogue);
            return false;
        }

        npc.ShowDialogue(thankYouDialogue);
        DropReward(npc.transform.position);
        hasBuyedItem = true;
        return true;
    }

    private void DropReward(Vector3 npcPosition)
    {
        GameObject droppedCoin = Instantiate(rewardPrefab, npcPosition + Vector3.down * 2, Quaternion.identity);

        Item coinItem = droppedCoin.GetComponent<Item>();
        coinItem.quantity = rewardQuantity;
        coinItem.UpdateQuantityDisplay();

        BounceEffect bounce = droppedCoin.GetComponent<BounceEffect>();
        if (bounce != null)
        {
            bounce.StartBounce();
        }
    }
}
using UnityEngine;

public class FishItem : Item
{
    public override void UseItem()
    {
        NPC nearbyNpc = FindNearbyNPC();

        if (nearbyNpc == null)
        {
            Debug.Log("Não há ninguém por perto.");
            return;
        }

        ItemBuyer buyer = nearbyNpc.GetComponent<ItemBuyer>();

        if (buyer != null && buyer.CanBuy(ID))
        {
            if (buyer.TrySell(nearbyNpc))
            {
                ConsumeOne();
            }
        }
        else
        {
            nearbyNpc.ShowDialogue(nearbyNpc.fishRejectionDialogue);
        }
    }
}
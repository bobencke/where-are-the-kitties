using UnityEngine;

public class StrawberryMoneyItem : Item
{
    public override void UseItem()
    {
        NPC nearbyNpc = FindNearbyNPC();

        if (nearbyNpc == null)
        {
            Debug.Log("Não há ninguém por perto.");
            return;
        }

        GiMorangoController gimorango = nearbyNpc.GetComponent<GiMorangoController>();

        if (gimorango == null)
        {
            nearbyNpc.ShowDialogue(nearbyNpc.pescatoMoneyRejectionDialogue);
            return;
        }

        gimorango.TryGetStrawberry(nearbyNpc);
    }
}

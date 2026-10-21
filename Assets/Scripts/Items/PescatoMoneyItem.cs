using UnityEngine;

public class PescatoMoneyItem : Item
{
    public override void UseItem()
    {
        NPC nearbyNpc = FindNearbyNPC();

        if (nearbyNpc == null)
        {
            Debug.Log("Não há ninguém por perto.");
            return;
        }

        PescatoController pescato = nearbyNpc.GetComponent<PescatoController>();

        if (pescato == null)
        {
            nearbyNpc.ShowDialogue(nearbyNpc.pescatoMoneyRejectionDialogue);
            return;
        }

        pescato.TryExchangeMoney(nearbyNpc);
    }
}
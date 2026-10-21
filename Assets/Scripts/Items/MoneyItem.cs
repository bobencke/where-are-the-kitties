using UnityEngine;

public class MoneyItem : Item
{
    public override void UseItem()
    {
        NPC nearbyNpc = FindNearbyNPC();

        if (nearbyNpc == null)
        {
            Debug.Log("Não há ninguém por perto.");
            return;
        }

        FlamingueteController flaminguete = nearbyNpc.GetComponent<FlamingueteController>();

        if (flaminguete == null)
        {
            nearbyNpc.ShowDialogue(nearbyNpc.pescatoMoneyRejectionDialogue);
            return;
        }

        flaminguete.TryGivePasta(nearbyNpc);
    }
}
using UnityEngine;

public class PastaItem : Item
{
    public override void UseItem()
    {
        NPC nearbyNpc = FindNearbyNPC();

        if (nearbyNpc == null)
        {
            Debug.Log("Não há ninguém por perto.");
            return;
        }

        RicotaController ricota = nearbyNpc.GetComponent<RicotaController>();

        if (ricota == null)
        {
            nearbyNpc.ShowDialogue(nearbyNpc.pescatoMoneyRejectionDialogue);
            return;
        }

        ricota.TryExchangePasta(nearbyNpc);
    }
}

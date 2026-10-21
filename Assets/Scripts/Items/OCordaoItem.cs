using UnityEngine;

public class OCordaoItem : Item
{
    public override void UseItem()
    {
        NPC nearbyNpc = FindNearbyNPC();

        if (nearbyNpc == null)
        {
            Debug.Log("Não há ninguém por perto.");
            return;
        }

        FionaController fiona = nearbyNpc.GetComponent<FionaController>();

        if (fiona == null)
        {
            nearbyNpc.ShowDialogue(nearbyNpc.pescatoMoneyRejectionDialogue);
            return;
        }

        fiona.TryExchangeOCordao(nearbyNpc);
    }
}

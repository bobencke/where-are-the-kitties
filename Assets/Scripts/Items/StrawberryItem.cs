using UnityEngine;

public class StrawberryItem : Item
{
    public override void UseItem()
    {
        NPC nearbyNpc = FindNearbyNPC();

        if (nearbyNpc == null)
        {
            Debug.Log("Não há ninguém por perto.");
            return;
        }

        NaginiController nagini = nearbyNpc.GetComponent<NaginiController>();

        if (nagini == null)
        {
            nearbyNpc.ShowDialogue(nearbyNpc.pescatoMoneyRejectionDialogue);
            return;
        }

        nagini.TryGiveStrawberry(nearbyNpc);
    }
}

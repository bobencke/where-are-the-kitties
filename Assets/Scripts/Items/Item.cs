using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;
    public int quantity = 1;
    public int maxQuantity = 3;
    public float itemRadius = 2f;

    public TMP_Text quantityText;

    public virtual void UseItem()
    {
        Debug.Log($"Usando item {Name}");
        ConsumeOne();
    }

    public void ReduceQuantity(int amount)
    {
        quantity -= amount;

        if (quantity <= 0)
        {
            Slot slot = GetComponentInParent<Slot>();
            if (slot != null)
            {
                slot.currentItem = null;
            }
            Destroy(gameObject);
        }
        else
        {
            UpdateQuantityDisplay();
        }
    }

    protected void ConsumeOne()
    {
        ReduceQuantity(1);
    }

    public virtual void Pickup()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if (ItemPickUpUIController.Instance != null)
        {
            ItemPickUpUIController.Instance.ShowItemPickup(Name, itemIcon);
        }
    }

    public void UpdateQuantityDisplay()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : string.Empty;
        }
    }

    protected NPC FindNearbyNPC()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(PlayerMovement.Instance.transform.position, itemRadius);

        NPC closestNpc = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            NPC npc = hit.GetComponent<NPC>();
            if (npc != null)
            {
                float distance = Vector2.Distance(PlayerMovement.Instance.transform.position, npc.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNpc = npc;
                }
            }
        }

        return closestNpc;
    }
}
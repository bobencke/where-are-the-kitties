using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    private ItemDictionary itemDictionary;

    public GameObject hotBarPanel;
    public GameObject slotPrefab;
    public int slotCount;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();
    }

    public bool AddItem(GameObject itemPrefab)
    {
        foreach (Transform slotTransform in hotBarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                return true;
            }
            else if (slot != null && slot.currentItem == itemPrefab)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null && item.quantity < item.maxQuantity)
                {
                    item.quantity++;
                    return true;
                }
            }
        }

        Debug.Log("O inventário está cheio!");
        return false;
    }

    public bool RemoveItem(int itemID, int quantityToRemove)
    {
        if (!HasItem(itemID, quantityToRemove))
        {
            return false;
        }

        foreach (Transform slotTransform in hotBarPanel.transform)
        {
            if (quantityToRemove <= 0)
            {
                break;
            }

            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null && item.ID == itemID)
                {
                    int removeFromThisSlot = Mathf.Min(item.quantity, quantityToRemove);
                    item.ReduceQuantity(removeFromThisSlot);
                    quantityToRemove -= removeFromThisSlot;
                }
            }
        }

        return true;
    }

    public bool HasItem(int itemID, int requiredQuantity)
    {
        int totalQuantity = 0;

        foreach (Transform slotTransform in hotBarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null && item.ID == itemID)
                {
                    totalQuantity += item.quantity;
                }
            }
        }

        return totalQuantity >= requiredQuantity;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTransform in hotBarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData
                {
                    itemID = item.ID,
                    slotIndex = slotTransform.GetSiblingIndex(),
                    quantity = item.quantity
                });
            }
        }
        return invData;
    }

    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        foreach (Transform child in hotBarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotBarPanel.transform);
        }

        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = hotBarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject itemObject = Instantiate(itemPrefab, slot.transform);
                    itemObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = itemObject;

                    Item item = itemObject.GetComponent<Item>();
                    if (item != null)
                    {
                        item.quantity = data.quantity > 0 ? data.quantity : 1;
                        item.UpdateQuantityDisplay();
                    }
                }
            }
        }
    }

    public int GetTotalItemCount()
    {
        int total = 0;

        foreach (Transform slotTransform in hotBarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null)
                {
                    total += item.quantity;
                }
            }
        }

        return total;
    }
}
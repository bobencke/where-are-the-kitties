using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotBarController : MonoBehaviour
{
    public static HotBarController Instance;

    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 2;

    private ItemDictionary itemDictionary;

    private Key[] hotbarKeys;

    private void Awake()
    {
        Instance = this;

        itemDictionary = FindFirstObjectByType<ItemDictionary>();

        hotbarKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            hotbarKeys[i] = i < 9 ? (Key)((int)Key.Digit1 + i) : Key.Digit0;
        }
    }

    void Update()
    {
        if (PauseController.IsGamePaused || LoadingScreenController.IsTransitioning)
        {
            return;
        }

        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
                UseItemInSlot(i);
            }
        }
    }

    void UseItemInSlot(int index)
    {
        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
        if (slot.currentItem != null)
        {
            Item item = slot.currentItem.GetComponent<Item>();
            item.UseItem();
        }
    }

    public bool AddItem(GameObject itemPrefab)
    {
        Item prefabItem = itemPrefab.GetComponent<Item>();

        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem != null)
            {
                Item existingItem = slot.currentItem.GetComponent<Item>();
                if (existingItem != null && existingItem.ID == prefabItem.ID && existingItem.quantity < existingItem.maxQuantity)
                {
                    existingItem.quantity++;
                    existingItem.UpdateQuantityDisplay();
                    return true;
                }
            }
        }

        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                newItem.GetComponent<Item>().UpdateQuantityDisplay();
                return true;
            }
        }

        Debug.Log("O inventário está cheio!");
        return false;
    }

    public bool HasItem(int itemID, int requiredQuantity)
    {
        int totalQuantity = 0;

        foreach (Transform slotTransform in hotbarPanel.transform)
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

    public bool RemoveItem(int itemID, int quantityToRemove)
    {
        if (!HasItem(itemID, quantityToRemove))
        {
            return false;
        }

        foreach (Transform slotTransform in hotbarPanel.transform)
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

    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                hotbarData.Add(new InventorySaveData
                {
                    itemID = item.ID,
                    slotIndex = slotTransform.GetSiblingIndex(),
                    quantity = item.quantity
                });
            }
        }
        return hotbarData;
    }

    public void SetHotbarItems(List<InventorySaveData> hotbarSaveData)
    {
        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }

        foreach (InventorySaveData data in hotbarSaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
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

        foreach (Transform slotTransform in hotbarPanel.transform)
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
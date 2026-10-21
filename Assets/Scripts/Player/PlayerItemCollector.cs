using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private HotBarController hotbarController;

    void Start()
    {
        hotbarController = FindFirstObjectByType<HotBarController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if(item != null)
            {
                bool itemAdded = hotbarController.AddItem(collision.gameObject);

                if(itemAdded)
                {
                    item.Pickup();
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}

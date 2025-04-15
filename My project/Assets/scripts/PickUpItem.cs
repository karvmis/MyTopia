using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    // Reference to ItemData that is set in the Inspector for each item prefab
    public ItemData itemData;

    // Optional: Set the pickup range (adjust the value in the Inspector)
    public float pickupRange = 3f;


    void Update()
    {
        // Check if player is within the pickup range
        if (IsPlayerInRange())
        {
            // Check if 'E' is pressed when the player is within range
            if (Input.GetKeyDown(KeyCode.E))
            {
                AddItemToInventory();
                Destroy(gameObject); // Destroy the item from the scene after it is picked up
            }
        }
    }

    // Checks if the player is within the pickup range
    private bool IsPlayerInRange()
    {
        // Get the player's position
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Check the distance between the player and this item
        if (player != null && Vector3.Distance(player.transform.position, transform.position) <= pickupRange)
        {
            // If the player is within the pickup range
            return true;
        }

        return false;
    }

    // Add the item to the inventory
    private void AddItemToInventory()
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            inventory.AddItem(itemData);
        }
    }
}

using UnityEngine;

public class Inventory : MonoBehaviour
{
    public ItemData[] items = new ItemData[9];  // Inventory array to hold up to 9 items

    public void AddItem(ItemData itemToAdd)
    {
        // Looks for an empty slot in the inventory to add the item
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = itemToAdd;
                break;
            }
        }
    }
}

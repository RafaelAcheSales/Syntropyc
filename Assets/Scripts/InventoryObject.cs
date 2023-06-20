using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Object", menuName = "Inventory/Inventory Object")]
public class InventoryObject : ScriptableObject
{
    public ItemObject item;
    public int amount;

    public bool AddItem(ItemObject newItem, int amount)
    {
        if (newItem == null)
        {
            Debug.LogWarning("Cannot add null item to inventory.");
            return false;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("Amount must be a positive value.");
            return false;
        }

        if (item != null && newItem != item)
        {
            Debug.LogWarning("Cannot add a different item to a non-empty slot.");
            return false;
        }

        this.item = newItem;
        this.amount += amount;
        return true;
    }

    public void RemoveItem(ItemObject newItem, int amount)
    {
        if (newItem == null)
        {
            Debug.LogWarning("Cannot remove null item from inventory.");
            return;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("Amount must be a positive value.");
            return;
        }

        if (item != newItem)
        {
            Debug.LogWarning("Item to be removed is not in the inventory.");
            return;
        }

        if (this.amount < amount)
        {
            Debug.LogWarning("Not enough items in the inventory to remove.");
            return;
        }

        this.amount -= amount;

        if (this.amount == 0)
        {
            this.item = null;
        }
    }

    public void DropItems()
    {
        if (item == null)
        {
            Debug.LogWarning("No items in the inventory to drop.");
            return;
        }

        // Code to drop the items goes here

        this.item = null;
        this.amount = 0;
    }
}

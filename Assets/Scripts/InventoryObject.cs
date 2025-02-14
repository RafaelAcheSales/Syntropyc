using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Object", menuName = "Inventory/Inventory Object")]
public class InventoryObject : ScriptableObject
{
    public ItemObject item;
    public int amount;

    public bool AddItem(ItemObject newItem, int amount, GameObject itemPrefab)
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
            
            return false;
        }



        this.item = newItem;
        this.amount += amount;
        //Debug.Log("Added " + amount + " " + newItem.name + " to inventory. Total: " + this.amount);
        Destroy(itemPrefab);

        return true;
    }

    public GameObject RemoveItem(ItemObject newItem, int amount)
    {
        if (newItem == null)
        {
            Debug.LogWarning("Cannot remove null item from inventory.");
            return null;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("Amount must be a positive value.");
            return null;
        }

        if (item != newItem)
        {
            Debug.LogWarning("Item to be removed is not in the inventory.");
            return null;
        }

        if (this.amount < amount)
        {
            Debug.LogWarning("Not enough items in the inventory to remove.");
            return null;
        }

        this.amount -= amount;
        if (this.amount == 0)
        {
            this.item = null;
        }
        return null;
    }

    public GameObject DropItems()
    {
        if (item == null)
        {
            Debug.LogWarning("No items in the inventory to drop.");
            return null;
        }

        // Code to drop the items goes here
        this.item = null;
        this.amount = 0;
        return null;
    }
}

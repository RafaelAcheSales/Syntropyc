using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Object", menuName = "Inventory/Inventory Object")]
public class InventoryObject : ScriptableObject
{
    public ItemObject item;
    public GameObject prefab;
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
            //Debug.LogWarning("Cannot add a different item to a non-empty slot.");
            return false;
        }

        this.item = newItem;
        this.amount += amount;
        this.prefab = itemPrefab;
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
        GameObject oldPrefab = this.prefab;
        if (this.amount == 0)
        {
            this.item = null;
            this.prefab = null;
        }
        return oldPrefab;
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
        GameObject oldPrefab = this.prefab;
        this.prefab = null;
        return oldPrefab;
    }
}

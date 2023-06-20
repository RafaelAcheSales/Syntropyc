using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public InventoryObject inventory;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PickableItem itemWorld)) return;
        if (!inventory.AddItem(itemWorld.item, 1)) return;
        Destroy(other.gameObject);
    }
}

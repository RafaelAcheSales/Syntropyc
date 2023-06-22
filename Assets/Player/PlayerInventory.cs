using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public InventoryObject inventory;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PickableItem itemWorld)) return;
        if (!inventory.AddItem(itemWorld.item, 1, itemWorld.gameObject)) return;
        itemWorld.gameObject.SetActive(false);
    }

    public void DropItems()
    {
        GameObject droppedItem;
        for (int i = 0; i < inventory.amount; i++)
        {
            Vector3 offset = GameManager.instance.GetMouseScreenPos().normalized;
            offset.z = 0f;
            droppedItem = Instantiate(inventory.prefab, transform.position + offset, Quaternion.identity);
            droppedItem.SetActive(true);
            droppedItem.GetComponent<PickableItem>().item = inventory.item;
            droppedItem.GetComponent<PickableItem>().SetPickupTimeout();
        }
        inventory.DropItems();
    }
}

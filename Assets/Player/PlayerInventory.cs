using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public InventoryObject inventoryBase;
    public InventoryObject inventory;
    public GameObject pickableItem;
    public float waterReserve = 10f;


    private void Start()
    {
        inventory = ScriptableObject.CreateInstance<InventoryObject>();
        pickableItem = Resources.Load<GameObject>("Items/PickableItem");
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PickableItem itemWorld)) return;
        if (!enabled) return;
        //Debug.Log(other.gameObject.name + " is in range.");
        if (!inventory.AddItem(itemWorld.item, 1, itemWorld.gameObject)) return;
        GameManager.instance.PlaySound(GameManager.instance.grabItemSound);
        //Debug.Log(other.gameObject.name + " has been added to inventory.");
        itemWorld.gameObject.SetActive(false);
    }

    public void DropItems()
    {
        GameObject droppedItem;
        //Debug.Log("Dropping " + inventory.amount + " ");
        for (int i = 0; i < inventory.amount; i++)
        {
            Vector3 offset = GameManager.instance.GetMouseScreenPos().normalized;
            offset.z = 0f;
            droppedItem = Instantiate(pickableItem, transform.position + offset, Quaternion.identity);
            droppedItem.GetComponent<PickableItem>().item = inventory.item;
            droppedItem.SetActive(true);
            droppedItem.GetComponent<PickableItem>().SetPickupTimeout();
        }
        inventory.DropItems();
    }
}

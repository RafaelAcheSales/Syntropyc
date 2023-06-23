using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoUpdateInventory : MonoBehaviour
{
    private PlayerInventory playerInventory;

    public Image itemImage;
    public TextMeshProUGUI itemAmount;
    public TextMeshProUGUI itemName;

    private Sprite defaultSprite;
    private string defaultName;

    private void Start()
    {
        playerInventory = GameManager.instance.playerInventory;
        defaultSprite = itemImage.sprite;
        defaultName = itemName.text;
    }

    private void Update()
    {
        if (!playerInventory.enabled) return;
        if (playerInventory.inventory.item == null)
        {
            itemImage.sprite = defaultSprite;
            itemAmount.text = " ";
            itemName.text = defaultName;
        } else {
            itemImage.sprite = playerInventory.inventory.item.sprite;
            itemAmount.text = playerInventory.inventory.amount.ToString();
            itemName.text = playerInventory.inventory.item.itemName;

        }
    }
}

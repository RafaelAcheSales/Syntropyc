using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Seed,
    Food,
    Fertilizer,
}

[CreateAssetMenu(fileName = "New Item Object", menuName = "Item/Item Object")]
public class ItemObject : ScriptableObject { 

    public string itemName;
    public string description;
    public Sprite sprite;
    public ItemType type;

}

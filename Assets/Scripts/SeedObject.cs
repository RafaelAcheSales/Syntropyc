using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Seed Object", menuName = "Items/Seed Object")]
public class SeedObject : ItemObject {
    public PlantaBase plantaBase;
    private void Awake()
    {
        type = ItemType.Seed;
    }
}

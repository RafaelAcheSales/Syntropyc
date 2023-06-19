using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Plant", menuName = "ScriptableObjects/NewPlant", order = 1)]
public class PlantaBase : ScriptableObject
{
    public  enum PlantLayer {
        High, 
        Medium,
        Low
    }
    public string plantName;
    public int maximunTimeToGrow;

    public int minimunTimeToGrow {
        get
        {
            return this.maximunTimeToGrow / 2;
        }
        set
        {
            return;
        }
    }

    public PlantLayer plantLayer;

    public Sprite[] sprites = new Sprite[3];

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Plant", menuName = "ScriptableObjects/NewPlant", order = 1)]
public class PlantaBase : ScriptableObject
{
    public  enum PlantLayer {
        Low = 0,
        Medium = 1,
        High = 2
    }
    public string plantName;
    public float maximunTimeToGrow;

    public float minimunTimeToGrow {
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

    public string plantPrefabPath = "Plant";
    public Sprite[] stageOneVisual = new Sprite[2];
    public Sprite[] stageTwoVisual = new Sprite[2];
    public Sprite[] stageThreeVisual = new Sprite[2];

 
    [SerializeField] public GrowthStage[] growthStages = new GrowthStage[3];
}
[System.Serializable]
public class GrowthStage
{
    public bool[] row0 = new bool[3];
    public bool[] row1 = new bool[3];
    public bool[] row2 = new bool[3];
}


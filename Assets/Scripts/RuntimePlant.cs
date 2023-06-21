using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlantStage
{
    StageOne,
    StageTwo,
    StageThree
}

public class RuntimePlant : MonoBehaviour
{
    public float photosynthesisScore = 0f;
    public PlantaBase plant { get; set; }
    public CustomTile currentTile { get; set; }
    private float currentDevelopment = 0f; //seconds
    private Branch initalBranch;
    public bool enableGrowth = false;
    public PlantStage currentStage = PlantStage.StageOne;
    public Dictionary<PlantaBase.PlantLayer, List<Branch>> branches = new Dictionary<PlantaBase.PlantLayer, List<Branch>>();
    public void OnCreated()
    {
        InitializeBranchDict();
        initalBranch = GetComponentInChildren<Branch>();
        initalBranch.gameObject.SetActive(false);
        CreateAndAddBranch(BranchType.Trunk, PlantaBase.PlantLayer.Low, currentTile);
        enableGrowth = true;
    }

    void InitializeBranchDict()
    {
        branches.Add(PlantaBase.PlantLayer.Low, new List<Branch>());
        branches.Add(PlantaBase.PlantLayer.Medium, new List<Branch>());
        branches.Add(PlantaBase.PlantLayer.High, new List<Branch>());
    }

    public void CreateAndAddBranch(BranchType branchType, PlantaBase.PlantLayer layer, CustomTile parentTile)
    {
        GameObject newBranch = Instantiate(initalBranch.gameObject, transform);
        newBranch.SetActive(true);
        newBranch.transform.position = parentTile.transform.position;
        Branch branchComponent = newBranch.GetComponent<Branch>();
        branchComponent.SetVariables(this, branchType, layer);
        branches[layer].Add(branchComponent);
        parentTile.SetOccupied((int) layer, true);
    }

    public float timeToGrow(float syntropy)
    {
        return ExtensionMethods.Remap(syntropy, 0f, 1f, plant.minimunTimeToGrow, plant.maximunTimeToGrow);
    }

    public float GetDevelopmentPercentage()
    {
        return ExtensionMethods.Remap(currentDevelopment, 0f, timeToGrow(currentTile.GetSyntropy()), 0f, 1f);
    }

    public int GetCurrentPlantStage()
    {
        int stageIndex = ExtensionMethods.MapFloatToIntInterval(GetDevelopmentPercentage(), 0f, 1f, 0, 2);
        return stageIndex;
    }

    public void DevelopPlant(float tileSyntropy)
    {
        if (!enableGrowth) return;
        currentDevelopment += Time.deltaTime;
        if (currentDevelopment >= timeToGrow(tileSyntropy))
        {
            currentDevelopment = timeToGrow(tileSyntropy);
            enableGrowth = false;
        }
        CheckDevelopmentThreshold();
    }

    private void CheckDevelopmentThreshold()
    {
        PlantStage stageIndex = (PlantStage)GetCurrentPlantStage();
        //if stage has changed
        if (currentStage != stageIndex)
        {
            currentStage = stageIndex;
            GrowToNewStage();
        }
    }

    private void GrowToNewStage()
    {
        //check if it plant base can grow higher
        if ((int)plant.plantLayer < (int)currentStage) return;
        //gets the new stage
        GrowthStage newStage = plant.growthStages[(int)currentStage];
        //checks for colisions on CustomTile.isLayerOccupied based on adjacent tiles
        //if there is no colision, create a new branch
        
        
        
        

    }
}

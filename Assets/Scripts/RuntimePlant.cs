using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlantStage
{
    StageOne = 0,
    StageTwo = 1,
    StageThree = 2
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
        if (parentTile.IsOccupied((int)layer) && layer != PlantaBase.PlantLayer.Low) return;
        GameObject newBranch = Instantiate(initalBranch.gameObject, transform);
        Branch branchComponent = newBranch.GetComponent<Branch>();
        branchComponent.SetVariables(this, branchType, layer, parentTile);
        newBranch.name = branchType.ToString() + " " + layer.ToString();
        newBranch.SetActive(true);
        newBranch.transform.position = parentTile.transform.position + new Vector3(0f, 0f, (int)layer*-0.5f);
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
    private void Update()
    {
        UpdatePhotosynthesisScore();
        //CallUpdateOnBranches();
    }
    void CallUpdateOnBranches()
    {
        foreach (KeyValuePair<PlantaBase.PlantLayer, List<Branch>> layer in branches)
        {
            foreach (Branch branch in layer.Value)
            {
                branch.UpdateBranch();
            }
        }
    }
    public void DevelopPlant(float tileSyntropy)
    {
        if (enableGrowth)
        {
            currentDevelopment += Time.deltaTime * GameManager.instance.globalGrowMultiplier;
            if (currentDevelopment >= timeToGrow(tileSyntropy))
            {
                currentDevelopment = timeToGrow(tileSyntropy);
                enableGrowth = false;
            }
            CheckDevelopmentThreshold();
        }
    }

    private void UpdatePhotosynthesisScore()
    {
        float averageLightScore = CalculateAverageLightScore();
        photosynthesisScore = averageLightScore;
    }
    private float CalculateAverageLightScore()
    {
        Branch[] allBranches = GetComponentsInChildren<Branch>(includeInactive: false);
        //Debug.Log("Found " + allBranches.Length + " branches");
        float totalLightScore = 0f;
        foreach (Branch branch in allBranches)
        {
            totalLightScore += branch.lightScore;
            //Debug.Log("Branch light score: " + branch.lightScore);
        }
        float result = totalLightScore / allBranches.Length;
        //Debug.Log("Average light score: " + result);
        return result;
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
        Debug.Log("Grow to new stage");
        //check if it plant base can grow higher
        Debug.Log((int)plant.plantLayer + " " + (int)currentStage);
        Debug.Log(plant.plantLayer + " " + currentStage);
        if ((int)plant.plantLayer < (int)currentStage) return;
        //if upper space from current tile is occupied, return
        if (currentTile.IsOccupied((int)currentStage)) return;
        //gets the new stage
        GrowthStage newStage = plant.growthStages[(int)currentStage];
        //checks for colisions on CustomTile.isLayerOccupied based on adjacent tiles
        //if there is no colision, create a new branch
        for (int i = 0; i < newStage.row0.Length; i++)
        {
            if (!newStage.row0[i]) continue;
            CustomTile newTile = currentTile.GetAdjacentTile(ConvertGrowthStageRowToNeighbor(i, 0));
            CreateAndAddBranch(BranchType.Leaf, (PlantaBase.PlantLayer)currentStage, newTile);
        }
        for (int i = 0; i < newStage.row1.Length; i++)
        {
            if (!newStage.row1[i]) continue;
            CustomTile newTile = currentTile.GetAdjacentTile(ConvertGrowthStageRowToNeighbor(i, 1));
            BranchType branchType = (i == 1) ? BranchType.Trunk : BranchType.Leaf;
            CreateAndAddBranch(branchType, (PlantaBase.PlantLayer)currentStage, newTile);
        }
        for (int i = 0; i < newStage.row2.Length; i++)
        {
            if (!newStage.row2[i]) continue;
            CustomTile newTile = currentTile.GetAdjacentTile(ConvertGrowthStageRowToNeighbor(i, 2));
            CreateAndAddBranch(BranchType.Leaf, (PlantaBase.PlantLayer)currentStage, newTile);
        }


    }

    public static TileNeighbor ConvertGrowthStageRowToNeighbor(int index, int rowIndex) {
        //rowIndex 0 = row0 left side
        //rowIndex 1 = row1 middle 
        //rowIndex 2 = row2 right side
        switch (rowIndex)
        {
            case 0:
                switch (index)
                {
                    case 0:
                        return TileNeighbor.UpLeft;
                    case 1:
                        return TileNeighbor.Left;
                    case 2:
                        return TileNeighbor.DownLeft;
                    default:
                        return TileNeighbor.None;
                }
            case 1:
                switch (index)
                {
                    case 0:
                        return TileNeighbor.Up;
                    case 1:
                        return TileNeighbor.Middle;
                    case 2:
                        return TileNeighbor.Down;
                    default:
                        return TileNeighbor.None;
                }
            case 2:
                switch (index)
                {
                    case 0:
                        return TileNeighbor.UpRight;
                    case 1:
                        return TileNeighbor.Right;
                    case 2:
                        return TileNeighbor.DownRight;
                    default:
                        return TileNeighbor.None;
                }
            default:
                return TileNeighbor.None;
        }
    }
}

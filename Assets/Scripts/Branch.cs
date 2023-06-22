using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BranchType
{
    Trunk,
    Leaf
}

[RequireComponent(typeof(SpriteRenderer))]
public class Branch : MonoBehaviour
{
    private RuntimePlant motherPlant;
    private SpriteRenderer spriteRenderer;
    private BranchType branchType;
    private PlantaBase.PlantLayer layer;
    private CustomTile parentTile;
    public float lightScore = 0f;
    public void SetVariables(RuntimePlant motherPlant, BranchType branchType, PlantaBase.PlantLayer layer, CustomTile parentTile)
    {
        this.motherPlant = motherPlant;
        this.branchType = branchType;
        this.layer = layer;
        this.parentTile = parentTile;
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameManager.onLayerViewToggle += ToggleBranch;

    }

    private void OnDisable()
    {
        GameManager.onLayerViewToggle -= ToggleBranch;
    }

    public void ToggleBranch(int layer)
    {

        //Debug.Log("Toggling branch to " + layer);
        if (layer < 0)
        {
            spriteRenderer.enabled = true;
            return;
        }
        spriteRenderer.enabled = (PlantaBase.PlantLayer)layer == this.layer;
    }

    private void FixedUpdate()
    {

        if (motherPlant != null)
        {
            Sprite newSprite = GetCorrectVisual();
            if (ReferenceEquals(newSprite, spriteRenderer.sprite)) return;
            spriteRenderer.sprite = newSprite;
        }
        print(Time.frameCount);
        //UpdateLightScore();
    }

    private void UpdateLightScore()
    {

        float score = 0f;
        //if other layes are occupied on tile , light score is 1
        //Debug.Log("Checking light score for " + parentTile.name);
        List<PlantaBase.PlantLayer> TheOtherLayers = GameManager.LayerSetSubtraction(layer);
        //Debug.Log("The other layers are " + TheOtherLayers[0] + " and " + TheOtherLayers[1]);
        foreach (PlantaBase.PlantLayer layer in TheOtherLayers)
        {
            if (parentTile.IsOccupied((int)layer))
            {
                //Debug.Log("Layer " + layer + " is occupied");
                score += 0.5f;
            }
        }
        //Debug.Log("Branch Light score is " + score);
        lightScore = score;

    }

    Sprite GetCorrectVisual()
    {
        switch (motherPlant.currentStage)
        {
            case PlantStage.StageTwo:
                return motherPlant.plant.stageTwoVisual[(int)branchType];
            case PlantStage.StageThree:
                return motherPlant.plant.stageThreeVisual[(int)branchType];
            default:
                return motherPlant.plant.stageOneVisual[(int)branchType];
        }
    }
}

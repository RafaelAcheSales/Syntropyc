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
    public float lightScore = 0f;
    public void SetVariables(RuntimePlant motherPlant, BranchType branchType, PlantaBase.PlantLayer layer)
    {
        this.motherPlant = motherPlant;
        this.branchType = branchType;
        this.layer = layer;
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

    private void Update()
    {
        if (!motherPlant) return;
        Sprite newSprite = GetCorrectVisual();
        if (ReferenceEquals(newSprite, spriteRenderer.sprite)) return;
        spriteRenderer.sprite = newSprite;
        UpdateLightScore();
    }

    private void UpdateLightScore()
    {

        float score = 0f;
        //if other layes are occupied on tile , light score is 1
        List<PlantaBase.PlantLayer> TheOtherLayers = GameManager.LayerSetSubtraction(layer);
        foreach (PlantaBase.PlantLayer layer in TheOtherLayers)
        {
            if (motherPlant.currentTile.IsOccupied((int)layer))
            {
                score += 0.5f;
            }
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoUpdateResources : MonoBehaviour
{
    public RectTransform waterLevel;
    public RectTransform compostLevel;

    private void Update()
    {
        UpdateWaterLevel();
        UpdateCompostLevel();
    }
    public void UpdateWaterLevel()
    {
        float newWaterLevel = GameManager.instance.waterReserve / GameManager.instance.waterReserveMax;
        waterLevel.localScale = new Vector3(1, newWaterLevel, 1);
    }

    public void UpdateCompostLevel()
    {
        float newCompostLevel = GameManager.instance.compostReserve / GameManager.instance.compostReserveMax;
        compostLevel.localScale = new Vector3(1, newCompostLevel, 1);
    }
}

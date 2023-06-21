using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomTile : MonoBehaviour
{
    //public 

    public float syntropy = 0f;
    public float lightLevel = 0f;

    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    
    private RuntimePlant plant;

    public bool[] layerIsOccupied = new bool[3] { false, false, false };
    void Start()
    {
        syntropy = Random.Range(0f, 1f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();

    }
    private void Update()
    {
        if (plant != null)
        {
            plant.DevelopPlant(syntropy);
        }
    }

    public void UpdateSprite()
    {
        float remapped = ExtensionMethods.Remap(syntropy, 0f, 1f, 0f, sprites.Length);
        if (remapped >= sprites.Length)
        {
            remapped = sprites.Length - 1;
        }
        int index = Mathf.FloorToInt(remapped);
        spriteRenderer.sprite = sprites[index];
    }

    public bool IsOccupied(int layer)
    {
        return layerIsOccupied[layer];
    }

    public void SetOccupied(int layer, bool value)
    {
        layerIsOccupied[layer] = value;
    }
    public float GetSyntropy()
    {
        return syntropy;
    }

    public string GetFormattedInfo()
    {
        return $"Syntropy: {syntropy}\nLight Level: {lightLevel}\nGrowth: {(GetPlantDevelopmentInfo()*100).ToString()}";
    }

    float GetPlantDevelopmentInfo()
    {
        if (plant == null) return 0f;
        return plant.GetDevelopmentPercentage();

    }

    public bool TryPlant(SeedObject seed)
    {
        if (IsOccupied(0)) return false;
        GameObject plantPrefab = Resources.Load<GameObject>(seed.plantaBase.plantPrefabPath);
        plant = Instantiate(plantPrefab, transform.position, Quaternion.identity).GetComponent<RuntimePlant>();
        plant.plant = seed.plantaBase;
        plant.transform.parent = transform;
        plant.currentTile = this.gameObject.GetComponent<CustomTile>();
        SetOccupied(0, true);
        plant.OnCreated();
        return true;

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileNeighbor
{
    Middle,
    Up,
    Down,
    Left,
    Right,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight,
    None
}

public class CustomTile : MonoBehaviour
{
    //public 

    public float syntropy = 0f;
    public float compost = 0f;
    public float water = 0f;
    public float compoundLostPerSecond = 0.005f;
    public float waterLostPerSecond = 0.01f;

    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    
    private RuntimePlant plant;

    public bool[] layerIsOccupied = new bool[3] { false, false, false };
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();

    }
    private void Update()
    {
        if (plant != null)
        {
            plant.DevelopPlant(syntropy);
        }
        CalculateCurrentSyntropy();
        LoseWaterAndCompost();
        UpdateSprite();

    }
    private void CalculateCurrentSyntropy()
    {
        if (plant == null)
        {
            syntropy = compost * 0.3f + water * 0.3f;
        } else
        {
            syntropy = compost * 0.3f + water * 0.3f + plant.photosynthesisScore*0.4f;
        }
    }
    void LoseWaterAndCompost()
    {
        SubtractWater(waterLostPerSecond * Time.deltaTime);
        SubtractCompost(compoundLostPerSecond * Time.deltaTime);
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
    public void AddWater(float value)
    {
        water += value;
        if (water > 1f) water = 1f;
    }
    public void SubtractWater(float value)
    {
        water -= value;
        if (water < 0f) water = 0f;
    }

    public void AddCompost(float value)
    {
        compost += value;
        if (compost > 1f) compost = 1f;
    }

    public void SubtractCompost(float value)
    {
        compost -= value;
        if (compost < 0f) compost = 0f;
    }

    public string GetFormattedInfo()
    {
        return $"Syntropy: {syntropy}\nLight Level: {GetPlantLightLevel()}\nWater: {water}\nCompost: {compost}\nGrowth: {(GetPlantDevelopmentInfo()*100).ToString()}%";
    }

    float GetPlantDevelopmentInfo()
    {
        if (plant == null) return 0f;
        return plant.GetDevelopmentPercentage();

    }
    float GetPlantLightLevel()
    {
        if (plant == null) return 0f;
        return plant.photosynthesisScore;
    }

    public void Highlight(bool state)
    {
        spriteRenderer.color = state ? Color.green : Color.white;
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

    public CustomTile GetAdjacentTile(TileNeighbor neighbor)
    {
        Vector2Int offset = Vector2Int.zero;
        switch (neighbor)
        {
            case TileNeighbor.Up:
                offset = Vector2Int.up;
                break;
            case TileNeighbor.Down:
                offset = Vector2Int.down;
                break;
            case TileNeighbor.Left:
                offset = Vector2Int.left;
                break;
            case TileNeighbor.Right:
                offset = Vector2Int.right;
                break;
            case TileNeighbor.UpLeft:
                offset = Vector2Int.up + Vector2Int.left;
                break;
            case TileNeighbor.UpRight:
                offset = Vector2Int.up + Vector2Int.right;
                break;
            case TileNeighbor.DownLeft:
                offset = Vector2Int.down + Vector2Int.left;
                break;
            case TileNeighbor.DownRight:
                offset = Vector2Int.down + Vector2Int.right;
                break;
            case TileNeighbor.Middle:
                offset = Vector2Int.zero;
                break;
            case TileNeighbor.None:
                return null;
        }
        Vector2Int newCoords = new Vector2Int((int)transform.position.x, (int)transform.position.y) + offset;
        return GameManager.instance.grid.GetTile(newCoords);
    }
}

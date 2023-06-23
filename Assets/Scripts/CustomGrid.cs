using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class CustomGrid : MonoBehaviour
{
    public int sizeX, sizeY;
    //public int cellSize; //pixels
    public GameObject tilePrefab;
    public int minimumTilesToFinish = 60;
    public float probabilityOfWaterTile = 0.1f;
    private Dictionary<Vector2, CustomTile> gridDictionary = new();
    private void Start()
    {
        GenerateGrid();
    }
    void GenerateGrid()
    {
        int middleX = sizeX / 2;
        int middleY = sizeY / 2;
        
        for (int x = 0; x < sizeX; x += 1)
        {
            for (int y = 0; y < sizeY; y += 1)
            {
                //spawns relative to player pos
                Vector2 newPos = new(x - middleX, y - middleY);
                GameObject newTile = Instantiate(tilePrefab, new Vector3(newPos.x, newPos.y, 0), Quaternion.identity);
                newTile.transform.parent = transform;
                newTile.name = $"Tile {x}, {y}";
                CustomTile newTileScript = newTile.GetComponent<CustomTile>();
                gridDictionary.Add(newPos, newTileScript);
                float random = Random.Range(0f, 1f);
                if (random <= probabilityOfWaterTile)
                {
                    newTileScript.isWaterTile = true;
                    //Debug.Log($"Tile {x}, {y} is water");
                }
                newTileScript.StartTile();
            }
        }   
    }

    public CustomTile GetTile(Vector2 pos)
    {
        if (gridDictionary.ContainsKey(pos))
        {
            return gridDictionary[pos];
        }
        else
        {
            return null;
        }
    }
    // from 0f to 1f
    public float GlobalSyntropyPercentage()
    {
        float totalSyntropy = 0;
        foreach (KeyValuePair<Vector2, CustomTile> item in gridDictionary)
        {
            totalSyntropy += item.Value.syntropy;
        }
        return totalSyntropy / minimumTilesToFinish;
    }
    
}

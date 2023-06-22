using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CustomGrid : MonoBehaviour
{
    public int sizeX, sizeY;
    //public int cellSize; //pixels
    public GameObject tilePrefab;
    public int minimumTilesToFinish = 60;
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
                gridDictionary.Add(newPos, newTile.GetComponent<CustomTile>());
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

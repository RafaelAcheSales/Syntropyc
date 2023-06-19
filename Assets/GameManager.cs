using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap[] tileMaps;
    public static GameManager instance;
    public Grid grid;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start() {
        grid = GetComponent<Grid>();
        ArrayList tileMaps = new ArrayList();
        foreach (Transform child in transform)
        {
            if (child.gameObject.TryGetComponent(out Tilemap tilemap))
            {
                tileMaps.Add(tilemap);
            }

        }
        this.tileMaps = (Tilemap[]) tileMaps.ToArray(typeof(Tilemap));
    }

    
}

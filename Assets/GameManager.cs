using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap[] tileMaps;
    public static GameManager instance;
    public Grid grid;
    public Camera mainCamera;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start() {
        grid = GetComponent<Grid>();
        mainCamera = Camera.main;
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

    private void Update() {
        Vector3 mousePos = Input.mousePosition;
        mousePos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -10f));
        Debug.DrawLine(Vector3.zero, mousePos, Color.red);
    }
    
}

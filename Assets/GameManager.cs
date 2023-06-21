using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class GameManager : MonoBehaviour
{
    public Tilemap[] tileMaps;
    public static GameManager instance;
    public CustomGrid grid;
    public Camera mainCamera;
    public GameObject tooltipPrefab;
    public GameObject inventoryUI;
    public GameObject player;
    public PlayerInventory playerInventory;
    public int amountOfEachDebugItem = 5;
    public GameObject[] debugItems;
    private Vector2 offset;
    private bool enableTooltip = true;

    private int currentViewLayer = -1;

    // event delegate to hide/show branches for the currentViewLayer
    public delegate void OnLayerViewToggle(int layer);

    public static event OnLayerViewToggle onLayerViewToggle;

    public static List<PlantaBase.PlantLayer> LayerSetSubtraction(PlantaBase.PlantLayer myLayer)
    {
        List<PlantaBase.PlantLayer> layers = new List<PlantaBase.PlantLayer>();
        foreach (PlantaBase.PlantLayer layer in System.Enum.GetValues(typeof(PlantaBase.PlantLayer)))
        {
            if (layer != myLayer)
            {
                layers.Add(layer);
            }
        }
        return layers;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerInventory = player.GetComponent<PlayerInventory>();
    }

    private void Start()
    {

        mainCamera = Camera.main;
        grid = FindObjectOfType<CustomGrid>();

        offset = new Vector2(150, 150);

#if UNITY_EDITOR
        SpawnDebugItems();
#endif
    }
    private void Update()
    {
        CustomTile tile = GetTileAtMousePos();
        ShowTileInfo(tile);
        CheckToggleTooltip();
        CheckInventoryAction();
        HandleLayerViewToggle();
        HandleClick(tile);
        HandleWater(tile);
        HanleCompostDebug(tile);
        //HandleCut();
    }

    void SpawnDebugItems()
    {
        Vector3 playerPos = player.transform.position;
        
        List<SeedObject> debugSeeds = Resources.LoadAll<SeedObject>("Items/Seeds").ToList();
        Debug.Log("debugSeeds: " + debugSeeds.Count);
        foreach (GameObject item in debugItems)
        {
            Debug.Log("item: " + item.name);
            if (item.TryGetComponent<PickableItem>(out PickableItem pickableItem))
            {
                for (int i = 0; i < amountOfEachDebugItem; i++)
                {
                    Vector3 itemOffset = new Vector3(Random.Range(-2, 2), Random.Range(-2, 2), 0);
                    //test if pickableItem.item is subclass SeedObject
                    GameObject spawnedItem = Instantiate(item, playerPos + itemOffset, Quaternion.identity);
                    int randomIndex = Random.Range(0, debugSeeds.Count);
                    spawnedItem.GetComponent<PickableItem>().item = debugSeeds[randomIndex];
                    spawnedItem.SetActive(true);
                    

                }
            }
        }
        
    }
    public Vector3 GetMouseScreenPos()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = 10;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(pos);
        return mousePos;
    }
    void ShowTileInfo(CustomTile tile)
    {
        if (tile != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(tooltipPrefab.transform.parent as RectTransform, Input.mousePosition, null, out Vector2 localPoint);
            ShowTooltipAt(localPoint, tile.GetFormattedInfo());
        }
        else
        {
            HideTooltip();
        }
    }

    public CustomTile GetTileAtMousePos()
    {
        Vector3 mousePos = GetMouseScreenPos();
        CustomTile tile = grid.GetTile(new Vector2(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y)));
        return tile;
    }

    void CheckToggleTooltip()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (enableTooltip)
            {
                enableTooltip = false;
                HideTooltip();
            }
            else
            {
                enableTooltip = true;
            }
        }
    }
    void CheckInventoryAction()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            player.GetComponent<PlayerInventory>().DropItems();
        }
    }

    void HandleLayerViewToggle()
    {
        bool hasChanged = false;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentViewLayer = 0;
            hasChanged = true;
            //TODO hide branches
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentViewLayer = 1;
            hasChanged = true;
            //TODO hide branches
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentViewLayer = 2;
            hasChanged = true;
            //TODO show branches
        } if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentViewLayer = -1;
            hasChanged = true;
            //TODO show branches
        }
        if (onLayerViewToggle == null || !hasChanged) return;
        onLayerViewToggle(currentViewLayer);
    }

    void HandleClick(CustomTile tile)
    {
        if (!Input.GetMouseButtonDown(0)) return;
        ItemObject currentItem = playerInventory.inventory.item;
        if (currentItem != null)
        {
            TryUseItem(currentItem, tile);

        } else
        {
            //TODO handle click without item
        }
        
    }

    void HandleWater(CustomTile tile)
    {
        if (!Input.GetKeyDown(KeyCode.F)) return;
        if (tile == null) return;
        tile.AddWater(0.3f);
        
    }
    void HanleCompostDebug(CustomTile tile)
    {
        if (!Input.GetKeyDown(KeyCode.C)) return;
        if (tile == null) return;
        tile.AddCompost(0.3f);
    }
    void TryUseItem(ItemObject currentItem, CustomTile tile)
    {
        switch (currentItem.type)
        {
            case ItemType.Seed:
                if (tile != null)
                {
                    SeedObject seed = currentItem as SeedObject;
                    if (seed != null)
                    {
                        if (tile.TryPlant(seed))
                        {
                            playerInventory.inventory.RemoveItem(currentItem, 1);
                        }
                    }
                }
                break;
        }
    }
    public void ShowTooltipAt(Vector2 pos, string text, float autoCloseTime = 0)
    {
        if (!enableTooltip) return;
        tooltipPrefab.SetActive(true);
        RectTransform rectTransform = tooltipPrefab.transform as RectTransform;
        rectTransform.localPosition = pos;
        tooltipPrefab.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = text;
        if (autoCloseTime > 0) StartCoroutine(AutoCloseTooltip(autoCloseTime));

    }

    public void HideTooltip()
    {
        tooltipPrefab.SetActive(false);
    }

    IEnumerator AutoCloseTooltip(float time)
    {
        yield return new WaitForSeconds(time);
        HideTooltip();
    }

    IEnumerator BlinkTile(CustomTile tile)
    {
        if (tile != null)
        {
            float alpha = tile.GetSyntropy();
            Color color = new Color(1, 1, 1, alpha);
            tile.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(1f);
            color = new Color(1, 1, 1, 1);
            tile.GetComponent<SpriteRenderer>().color = color;
            
        }
    }
}
        
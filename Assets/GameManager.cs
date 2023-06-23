using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap[] tileMaps;
    public static GameManager instance;
    public CustomGrid grid;
    public Camera mainCamera;
    public PlayableDirector playableDirector;
    public GameObject tooltipPrefab;
    public GameObject congratulationsEndText;
    public GameObject inventoryUI;
    public GameObject player;
    public PlayerInventory playerInventory;
    public PlayerActionAnimator playerActionAnimator;
    public RectTransform globalSyntropyBar;
    public AudioSource audioSource;
    public AudioClip endGameSound;
    public AudioClip cutSound;
    public AudioClip waterSound;
    public AudioClip grabItemSound;
    public AudioClip putCompostSound;
    public int amountOfEachDebugItem = 5;
    public GameObject[] debugItems;
    private Vector2 offset;
    private bool enableTooltip = true;
    private CustomTile highlightedTile;
    private bool updateEnabled = true;
    private int currentViewLayer = -1;
    public float waterReserve = 10f;
    public float waterReserveMax = 10f;
    public float compostReserve = 10f;
    public float compostReserveMax = 10f;
    public float globalGrowMultiplier = 1f;
    public RectTransform totalSyntropy;

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
        playerInventory = player.GetComponentInChildren<PlayerInventory>();
        playerActionAnimator = player.GetComponentInChildren<PlayerActionAnimator>();
        playerInventory.enabled = false;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {

        mainCamera = Camera.main;
        grid = FindObjectOfType<CustomGrid>();

        offset = new Vector2(150, 150);
        StartCoroutine(EnableInventory());

#if UNITY_EDITOR
#endif
        SpawnDebugItems();
    }

    IEnumerator EnableInventory()
    {
        yield return new WaitForSeconds(2f);
        playerInventory.enabled = true;
    }
    private void Update()
    {
        if (updateEnabled)
        {
            UpdateGame();
        }

    }
    void UpdateGame()
    {
        CustomTile tile = GetTileAtMousePos();
        ShowTileInfo(tile);
        CheckToggleTooltip();
        CheckInventoryAction();
        HandleLayerViewToggle();
        HandleClick(tile);
        HandleWater(tile);
        HanleCompostDebug(tile);
        Branch.allBranches.ForEach(branch => branch.UpdateBranch());
        UpdateGlobalSyntropyBar();
        HandleCut(tile);
    }
    void UpdateGlobalSyntropyBar()
    {
        float syntropy = grid.GlobalSyntropyPercentage();
        globalSyntropyBar.localScale = new Vector3(syntropy, 1, 1);
        if (syntropy >= 1)
        {
            EndGame();
        }
    }


    void EndGame()
    {
        Debug.Log("Game Over");
        updateEnabled = false;
        playableDirector.Play();
        //set inactive all canvas on scene
        Canvas[] canvas = FindObjectsOfType<Canvas>();
        foreach (Canvas c in canvas)
        {
            c.gameObject.SetActive(false);
        }
        congratulationsEndText.SetActive(true);
        audioSource.PlayOneShot(endGameSound);
    }
    public void CloseGame()
    {
        Debug.Log("Game Closed");
        UnityEngine.Application.Quit();
    }

    void SpawnDebugItems()
    {
        Vector3 playerPos = player.transform.position;
        
        List<SeedObject> debugSeeds = Resources.LoadAll<SeedObject>("Items/Seeds").ToList();
        //Debug.Log("debugSeeds: " + debugSeeds.Count);
        foreach (GameObject item in debugItems)
        {
            //Debug.Log("item: " + item.name);
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
            if (!ReferenceEquals(tile, highlightedTile) && highlightedTile != null)
            {
                highlightedTile.Highlight(false);
            }
            tile.Highlight(true);
            highlightedTile = tile;
        }
        else
        {
            HideTooltip();
            if (highlightedTile != null)
            {
                highlightedTile.Highlight(false);
                highlightedTile = null;
            }
        }
    }

    public CustomTile GetTileAtMousePos()
    {
        Vector3 mousePos = GetMouseScreenPos();
        CustomTile tile = grid.GetTile(new Vector2(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y)));
        return tile;
    }
    public GameObject CreatePickableObjectAt(Vector3 pos, ItemObject item, Transform parent = null)
    {
        GameObject pickableItem = Instantiate(debugItems[0], pos, Quaternion.identity, parent);
        pickableItem.GetComponent<PickableItem>().item = item;
        pickableItem.SetActive(true);
        return pickableItem;
    }
    public CustomTile GetTileAt(Vector3 pos)
    {
        CustomTile tile = grid.GetTile(new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y)));
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
            playerInventory.DropItems();
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
        if (tile.isWaterTile)
        {
            FillWater(tile);
        } else
        {
            if (waterReserve <= 0) return;
            float valueToAdd = Mathf.Min(waterReserve, 1f);
            tile.AddWater(valueToAdd);
            audioSource.PlayOneShot(waterSound);
            playerActionAnimator.WaterAction();
            waterReserve -= valueToAdd;
            waterReserve = Mathf.Clamp(waterReserve, 0f, waterReserveMax);

        }
        
    }
    void FillWater(CustomTile tile)
    {
        Debug.Log("FillWater");
        waterReserve = waterReserveMax;
        PlaySound(waterSound);

    }
    void HanleCompostDebug(CustomTile tile)
    {
        if (compostReserve <= 0) return;
        if (!Input.GetKeyDown(KeyCode.C)) return;
        if (tile == null) return;
        float valueToAdd = Mathf.Min(compostReserve, 1f);
        tile.AddCompost(valueToAdd);
        compostReserve -= valueToAdd;
        compostReserve = Mathf.Clamp(compostReserve, 0f, compostReserveMax);
        PlaySound(putCompostSound);
    }
    public void FillCompost()
    {
        compostReserve = Mathf.Min(compostReserveMax, compostReserve + 1);
    }
    void HandleCut(CustomTile tile)
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        Debug.Log("HandleCut on tile: " + tile.name);
        audioSource.PlayOneShot(cutSound);
        playerActionAnimator.ScyteAction();
        if (tile.TryCutGrass())
        {
            compostReserve = Mathf.Min(compostReserveMax, compostReserve + 1);
        }
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
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public string GetCurrentSeason()
    {
        string[] seasons = new string[] { "Pioneer", "Intermediary", "Abundance" };
        //map from 0f to 1f
        int index = ExtensionMethods.MapFloatToIntInterval(globalSyntropyBar.localScale.x, 0f, 1f, 0, seasons.Length);
        index = Mathf.Clamp(index, 0, seasons.Length - 1);
        return seasons[index];
    }
}
        
using System.Collections;
using System.Collections.Generic;
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
    private Vector2 offset;
    private bool enableTooltip = true;
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
    }

    private void Update()
    {
        ShowTileInfo();
        CheckToggleTooltip();
        CheckInventoryAction();
        HandleClick();
    }
    public Vector3 GetMouseScreenPos()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = 10;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(pos);
        return mousePos;
    }
    void ShowTileInfo()
    {
        CustomTile tile = GetTileAtMousePos();
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

    void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        CustomTile tile = GetTileAtMousePos();
        ItemObject currentItem = playerInventory.inventory.item;
        if (currentItem != null)
        {
            TryUseItem(currentItem, tile);

        } else
        {
            //TODO handle click without item
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
}
        
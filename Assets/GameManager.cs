using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap[] tileMaps;
    public static GameManager instance;
    public CustomGrid grid;
    public Camera mainCamera;
    public GameObject tooltipPrefab;

    public GameObject player;
    private Vector2 offset;
    private bool enableTooltip = true;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {

        mainCamera = Camera.main;
        grid = FindObjectOfType<CustomGrid>();
        player = GameObject.FindGameObjectWithTag("Player");
        offset = new Vector2(150, 150);
    }

    private void Update()
    {
        ShowTileInfo();
        CheckToggleTooltip();
    }

    void ShowTileInfo()
    {
        Vector3 pos = Input.mousePosition;
        pos.z = 10;
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(pos);
        CustomTile tile = grid.GetTile(new Vector2(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y)));
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
        
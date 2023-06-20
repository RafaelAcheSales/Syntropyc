using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTile : MonoBehaviour
{
    public float syntropy = 0f;
    public float lightLevel = 0f;

    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private 
    void Start()
    {
        syntropy = Random.Range(0f, 1f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();

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

    public float GetSyntropy()
    {
        return syntropy;
    }

    public string GetFormattedInfo()
    {
        return $"Syntropy: {syntropy}\nLight Level: {lightLevel}";
    }

    public bool TryPlant(SeedObject seed)
    {
        return false;
    }
}

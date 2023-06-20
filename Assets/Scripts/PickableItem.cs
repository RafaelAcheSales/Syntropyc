using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public ItemObject item;
    public SpriteRenderer spriteRenderer;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Awake()
    {
        spriteRenderer.sprite = item.sprite;
    }
}

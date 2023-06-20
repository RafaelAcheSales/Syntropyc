using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public ItemObject item;
    public SpriteRenderer spriteRenderer;
    public Collider2D currentCollider;
    public float pickupTimeout = 5f;
    public float fadeValue = 1f;
    public float minimumFadeValue = 0.4f;
    public float fadeRateSpeed = 1f;
    private bool isFading = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentCollider = GetComponent<Collider2D>();
        spriteRenderer.sprite = item.sprite;
    }



    public void SetPickupTimeout()
    {
        StartCoroutine(PickupTimeout(pickupTimeout));
    }
    private void Update()
    {
        //Fades the sprite
        if (isFading)
        {
            fadeValue -= Time.deltaTime * fadeRateSpeed;
            if (fadeValue <= minimumFadeValue)
            {
                isFading = false;
                fadeValue = minimumFadeValue;
            }
        } else
        {
            fadeValue += Time.deltaTime * fadeRateSpeed;
            if (fadeValue >= 1f)
            {
                isFading = true;
                fadeValue = 1f;
            }
        }
        spriteRenderer.color = new Color(1f, 1f, 1f, fadeValue);


    }

    private IEnumerator PickupTimeout(float timeout)
    {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(timeout);
        GetComponent<Collider2D>().enabled = true;
    }
}

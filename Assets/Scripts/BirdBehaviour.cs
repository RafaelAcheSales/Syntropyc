using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBehaviour : MonoBehaviour
{
    public float speed = 1f;
    public Vector3 startingPosition;
    public float distance = 10000f;
    public float propability = 0.01f;
    public Vector3 offset;
    public ItemObject[] seeds;
    private bool isFlying = false;
    private bool hasSeeds = true;

    private void Start()
    {
        startingPosition = transform.position;
        seeds = Resources.LoadAll<ItemObject>("Items/Seeds");
    }

    private void Update()
    {
        float random = Random.Range(0f, 1f);
        if (random <= propability*Time.deltaTime && !isFlying)
        {
            isFlying = true;
            StartCoroutine(Fly());
        }
    }

    IEnumerator Fly()
    {
        Debug.Log("Flying");
        float time = 0;
        float randomizeDroppingChange = Random.Range((distance / 2) * 0.8f, (distance / 2) * 1.2f);
        while (time < distance)
        {
            time += speed * Time.deltaTime;
            transform.position += Vector3.up * speed * Time.deltaTime;
            
            if (time > randomizeDroppingChange && hasSeeds)
            {
                Debug.Log("Dropping seeds");
                hasSeeds = false;
                Vector3 seedPos = transform.position;
                seedPos.z = 0f;
                GameObject pickableItem = GameManager.instance.CreatePickableObjectAt(seedPos, seeds[Random.Range(0, seeds.Length)]);
                pickableItem.GetComponent<PickableItem>().SetPickupTimeout();
            }
            yield return null;
        }
        transform.position = RandomizeStartingPos(offset, startingPosition);
        isFlying = false;
        hasSeeds = true;
    }

    Vector3 RandomizeStartingPos(Vector3 offset, Vector3 startingPos)
    {
        float randomX = Random.Range(-offset.x, offset.x);
        float randomY = Random.Range(-offset.y, offset.y);
        float randomZ = Random.Range(-offset.z, offset.z);
        return startingPos + new Vector3(randomX, randomY, randomZ);
    }



}

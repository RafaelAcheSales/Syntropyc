using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeasonMarker : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        textMesh.text = $"Season: {GameManager.instance.GetCurrentSeason()}";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScrollMouseZoom : MonoBehaviour
{
    public CinemachineFramingTransposer VirtualCamera;
    public float defaultZoom = -10f;
    public float maxZoom = -5f;
    public float minZoom = -20f;
    private float currentZoom = 0f;
    void Start()
    {
        VirtualCamera = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
        currentZoom = defaultZoom;
    }

    // Update is called once per frame
    void Update()
    {
        //scroll zoom with mouse
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0f) return;
        currentZoom -= scroll*2f;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        VirtualCamera.m_TrackedObjectOffset = new Vector3(0f, 0f, currentZoom);

        //
    }
}

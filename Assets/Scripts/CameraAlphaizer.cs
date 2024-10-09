using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraAlphaizer : MonoBehaviour
{
    private float hoverStart;
    private Color initialColor;
    private GameObject lastHovered;

    private void Update()
    {
        var cam = Camera.main;
        if (cam == null) return;

        // Cast ray from camera to where mouse is pointing
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit))
        {
            return;
        }

        if (hit.collider.gameObject.CompareTag("Player"))
            return;

        if (lastHovered != hit.collider.gameObject)
        {
            hoverStart = Time.time;
            if (lastHovered != null)
                lastHovered.GetComponent<Renderer>().material.color = initialColor;
            initialColor = hit.collider.gameObject.GetComponent<Renderer>().material.color;
            lastHovered = hit.collider.gameObject;
        }

        var targetColor = initialColor;
        targetColor.a = 0.1f;
        hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.Lerp(initialColor, targetColor, Time.deltaTime);
    }
}
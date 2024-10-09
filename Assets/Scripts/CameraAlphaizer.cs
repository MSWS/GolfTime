using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CameraAlphaizer : MonoBehaviour
{
    [SerializeField]
    [Range(0, 255)]
    private int hoverAlpha = 25;
    private float hoverAlphaF { get { return hoverAlpha / 255f; } }

    [SerializeField]
    protected float maxRaycastDistance = 30f;

    [SerializeField]
    [Tooltip("Delay before objects hovered over with mouse start fading into transparency.")]
    protected float fadeInDelay = 0.0f;

    [SerializeField]
    [Tooltip("Delay before objects hovered over with mouse reach the specified \"hoverAlpha\" transparency.")]
    protected float fadeInDuration = 1.0f;

    [SerializeField]
    [Tooltip("Delay before objects no longer hovered over with mouse start fading out of transparency.")]
    protected float fadeOutDelay = 0.0f;

    [SerializeField]
    [Tooltip("Delay before objects no longer hovered over with mouse reach the original transparency.")]
    protected float fadeOutDuration = 1.0f;

    private readonly List<ModifiedObject> modifiedObjects = new();

    private readonly GameObject? lastHoveredObject = null;

    private class ModifiedObject
    {
        private readonly CameraAlphaizer master;

        public readonly GameObject obj;
        public readonly Renderer renderer;
        public readonly Color originalColor;
        private readonly Color hoverColor;
        public float? lastHoverStart = null, lastHoverEnd = null;

        public ModifiedObject(GameObject obj, CameraAlphaizer master)
        {
            this.master = master;
            this.obj = obj;
            renderer = obj.GetComponent<Renderer>();
            originalColor = renderer.material.color;
            hoverColor = new Color(originalColor.r, originalColor.g, originalColor.b, master.hoverAlphaF);
            lastHoverStart = Time.time;
        }

        public void tick()
        {
            
        }
    }

    private void Update()
    {
        foreach (var obj in modifiedObjects) obj.tick();

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit, maxRaycastDistance, Physics.IgnoreRaycastLayer)) return;
        var hitObj = hit.collider.gameObject;
        if (hitObj == null) return;
        if (hitObj.CompareTag("Player")) return;

        var existing = modifiedObjects.FirstOrDefault(modObj => modObj.obj == hitObj);
        if (existing != null)
        {
            existing.lastHovered = Time.time;
            return;
        }

        var modObj = new ModifiedObject(hitObj, this);
        modifiedObjects.Add(modObj);
    }
}
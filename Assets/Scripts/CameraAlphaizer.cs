using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
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
    private readonly List<ModifiedObject> toRemove = new();

    private GameObject? lastHoveredObject = null;

    private class ModifiedObject
    {
        private readonly CameraAlphaizer master;

        public readonly GameObject obj;
        public readonly Renderer renderer;
        public readonly Color originalColor;
        private readonly Color hoverColor;
        private Color achievedColor;
        public float? lastHoverStart = null, lastHoverEnd = null;

        public ModifiedObject(GameObject obj, CameraAlphaizer master)
        {
            this.master = master;
            this.obj = obj;
            renderer = obj.GetComponent<Renderer>();
            originalColor = renderer.material.color;
            hoverColor = new Color(originalColor.r, originalColor.g, originalColor.b, master.hoverAlphaF);
            lastHoverStart = Time.time;
            lastHoverEnd = null;
        }

        public void tick()
        {
            if (lastHoverStart.HasValue)
            {
                var elapsed = Time.time - lastHoverStart.Value;
                if (elapsed >= master.fadeInDelay)
                {
                    var progress = Mathf.Clamp01((elapsed - master.fadeInDelay) / master.fadeInDuration);
                    renderer.material.color = Color.Lerp(originalColor, hoverColor, progress);
                    achievedColor = renderer.material.color;
                }
                return;
            }

            if (!lastHoverEnd.HasValue)
            {
                var elapsed = Time.time - lastHoverEnd.Value;
                if (elapsed >= master.fadeOutDelay)
                {
                    var progress = Mathf.Clamp01((elapsed - master.fadeOutDelay) / master.fadeOutDuration);
                    renderer.material.color = Color.Lerp(achievedColor, originalColor, progress);
                    if (progress >= 1.0f)
                    {
                        renderer.material.color = originalColor;
                        master.toRemove.Add(this);
                    }
                }
            }
        }
    }

    private void Update()
    {
        foreach (var obj in modifiedObjects) obj.tick();
        modifiedObjects.RemoveAll(modObj => toRemove.Contains(modObj));
        toRemove.Clear();

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        GameObject? hitObj = null;

        if (Physics.Raycast(ray, out var hit, maxRaycastDistance, Physics.IgnoreRaycastLayer | Physics.DefaultRaycastLayers))
            hitObj = hit.collider.gameObject;

        if (hitObj == null || hitObj.CompareTag("Player"))
        {
            unsetPreviousObj();
            lastHoveredObject = null;
            return;
        }

        if (hitObj == lastHoveredObject) return;

        unsetPreviousObj();
        lastHoveredObject = hitObj;

        var existing = modifiedObjects.FirstOrDefault(modObj => modObj.obj == hitObj);
        if (existing != null)
        {
            existing.lastHoverStart = Time.time;
            existing.lastHoverEnd = null;
            return;
        }

        var modObj = new ModifiedObject(hitObj, this);
        modifiedObjects.Add(modObj);
    }

    private void unsetPreviousObj()
    {
        if (lastHoveredObject == null) return;
        var existing = modifiedObjects.FirstOrDefault(modObj => modObj.obj == lastHoveredObject);
        if (existing == null) return;
        existing.lastHoverStart = null;
        existing.lastHoverEnd = Time.time;
    }
}
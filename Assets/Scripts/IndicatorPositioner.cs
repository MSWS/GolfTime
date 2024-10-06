using System;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorPositioner : MonoBehaviour {
  [SerializeField]
  private Transform player;

  private Vector3 playerOffset;

  private bool isVisible = false;

  private Renderer[] childRenderers;

  private void Start() { childRenderers = GetComponentsInChildren<Renderer>(); }

  private void Update() {
    var cam = Camera.main;
    if (Input.GetMouseButton(0)) {
      gameObject.transform.position = player.position + playerOffset;
      return;
    }

    if (cam == null) return;

    // Cast ray from camera to where mouse is pointing
    var ray = cam.ScreenPointToRay(Input.mousePosition);
    if (!Physics.Raycast(ray, out var hit)) {
      notOnPlayer();
      return;
    }

    var hitPoint = hit.point;
    if (!hit.collider.gameObject.CompareTag("Player")) {
      notOnPlayer();
      return;
    }

    onPlayer();
    gameObject.transform.position = hitPoint;
    // We moved the indicator, update rotation to face player
    gameObject.transform.LookAt(player);
  }

  private void notOnPlayer() {
    if (!isVisible) return;
    foreach (var r in childRenderers) r.enabled = false;
    isVisible = false;
  }

  private void onPlayer() {
    playerOffset = gameObject.transform.position - player.transform.position;
    if (isVisible) return;
    foreach (var r in childRenderers) r.enabled = true;
    isVisible = true;
  }
}
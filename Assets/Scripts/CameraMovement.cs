using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
  [SerializeField]
  private Transform playerPosition;

  [SerializeField]
  [Range(0, 0.1f)]
  private float smoothSpeed = 0.003f, rotationSpeed = 0.003f;

  private Vector3 initialOffset;

  void Start() { initialOffset = transform.position - playerPosition.position; }

  public Vector3 GetOffset() { return initialOffset; }

  public void UpdateOffset(Vector3 offset) { initialOffset = offset; }

  // Update is called once per frame
  void Update() {
    transform.position = Vector3.Lerp(transform.position,
      playerPosition.position + initialOffset, smoothSpeed);

    // Move camera above ground
    if (transform.position.y < initialOffset.y / 2)
      transform.position = new Vector3(transform.position.x,
        initialOffset.y / 2, transform.position.z);

    var quatLook =
      Quaternion.LookRotation(playerPosition.position - transform.position);

    var newRotation =
      Quaternion.Lerp(transform.rotation, quatLook, rotationSpeed);
    transform.rotation = newRotation;
  }
}
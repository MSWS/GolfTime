using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class CameraFollow : MonoBehaviour {
  [SerializeField]
  private Transform playerPosition;

  [SerializeField]
  [Range(0, 0.1f)]
  private float smoothSpeed = 0.003f, rotationSpeed = 0.003f;

  [SerializeField]
  private float sensitivityX = 2f, sensitivityY = 0.5f;

  [SerializeField]
  private bool invertX, invertY;

  private Vector3 initialOffset;
  private bool isRotating;

  private float rotationX, rotationY;

  private void Start() {
    initialOffset = transform.position - playerPosition.position;
  }

  private void Update() {
    // Check if the middle mouse button is pressed
    if (Input.GetKeyDown(KeyCode.Mouse2)) {
      // Lock and hide the cursor
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible   = false;
      isRotating       = true;
      rotationX        = Input.GetAxis("Mouse X");
      rotationY        = Input.GetAxis("Mouse Y");
    }

    if (Input.GetKeyUp(KeyCode.Mouse2)) {
      // Unlock and show the cursor when button is released
      Cursor.lockState = CursorLockMode.None;
      Cursor.visible   = true;
      isRotating       = false;
    }

    // Handle rotation if the middle mouse button is held down
    if (isRotating) {
      // Get mouse movement
      var mouseX = sensitivityX * Time.deltaTime;
      var mouseY = sensitivityY * Time.deltaTime;
      mouseX *= invertX ?
        rotationX - Input.GetAxis("Mouse X") :
        Input.GetAxis("Mouse X") - rotationX;
      mouseY *= invertY ?
        rotationY - Input.GetAxis("Mouse Y") :
        Input.GetAxis("Mouse Y") - rotationY;

      // Calculate new offset based on rotation around the player
      var rotation = Quaternion.Euler(mouseY, mouseX, 0);
      initialOffset = rotation * initialOffset;

      transform.position = playerPosition.position + initialOffset;
      transform.LookAt(playerPosition);
      return;
    }

    transform.position = Vector3.Lerp(transform.position,
      playerPosition.position + initialOffset, smoothSpeed);

    // Move camera above ground
    if (transform.position.y < initialOffset.y / 2) {
      var transform1 = transform;
      var position = transform1.position;
      position = new Vector3(position.x, initialOffset.y / 2, position.z);
      transform1.position = position;
    }

    var quatLook =
      Quaternion.LookRotation(playerPosition.position - transform.position);

    var newRotation =
      Quaternion.Lerp(transform.rotation, quatLook, rotationSpeed);
    transform.rotation = newRotation;
  }
}
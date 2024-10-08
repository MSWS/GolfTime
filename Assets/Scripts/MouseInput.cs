using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour {
  private enum InputScaling {
    LINEAR, QUADRATIC, CUBIC, SQRT,
  }

  /**
  * SETTINGS
  */
  [SerializeField]
  [Range(0.001f, 1f)]
  private float mouseSensitivity = 1 / 500f;

  [SerializeField]
  private InputScaling scaling = InputScaling.QUADRATIC;

  [SerializeField]
  private float minPower = 0.1f, maxPower = 1f;

  [SerializeField]
  [Range(1, 10000f)]
  private float powerScale = 10f;

  [SerializeField]
  private Gradient powerColor;

  [SerializeField]
  private Transform powerBar;

  [SerializeField]
  private Transform powerBarHolder;

  /**
  * CLASS VARIABLES
  */
  private float initialScale;

  private Color initialColor;

  private Vector2? downPos;

  private bool canceled;

  /**
   * CACHE
   */
  private Renderer powerBarRenderer;

  private Rigidbody rb;

  private void OnValidate() {
    if (powerBar == null) throw new System.Exception("power bar not set");
    if (powerBarHolder == null) throw new Exception("power bar holder not set");
    if (minPower > maxPower)
      throw new System.Exception("min power is greater than max power");
  }

  private void Start() {
    powerBarRenderer = powerBar.GetComponent<Renderer>();
    rb               = GetComponent<Rigidbody>();

    initialScale = powerBar.localScale.y;
    initialColor = powerBarRenderer.material.color;
  }

  // Manages mouse input
  private void Update() {
    if (Input.GetKeyDown(KeyCode.Mouse0) && powerBarRenderer.enabled) {
      downPos = Input.mousePosition;
      return;
    }

    if (downPos == null) {
      canceled = false;
      return;
    }

    // Allow user to cancel power bar
    if (Input.GetKeyDown(KeyCode.Mouse1)) {
      resetPowerBar();
      canceled = true;
      return;
    }

    var drag  = (Vector2)Input.mousePosition - downPos.Value;
    var scale = scaleInput(drag.magnitude);
    if (Input.GetKey(KeyCode.Mouse0) && !canceled) {
      var localScale = powerBar.localScale;
      localScale = new Vector3(localScale.x,
        initialScale + scale, localScale.z);
      powerBar.localScale = localScale;
      var transform1 = powerBar.transform;
      transform1.position =
        powerBarHolder.transform.position + transform1.up * scale;

      // Change color based on % power
      var percentPower = getPercentPower(scale);
      powerBarRenderer.material.color = powerColor.Evaluate(percentPower);
      return;
    }

    if (Input.GetKeyUp(KeyCode.Mouse0)) {
      if (canceled) {
        canceled = false;
        return;
      }

      // Apply force using powerBar's rotation and scale
      var force = powerBar.transform.up * (scale * -1 * powerScale);
      rb.AddForce(force, ForceMode.Impulse);
      resetPowerBar();
    }
  }

  private void resetPowerBar() {
    downPos = null;
    var localScale = powerBar.localScale;
    localScale = new Vector3(localScale.x, initialScale, localScale.z);
    powerBar.localScale = localScale;
    powerBar.transform.position = powerBarHolder.transform.position;
    powerBarRenderer.material.color = initialColor;
  }

  private float scaleInput(float input) {
    input *= mouseSensitivity;
    input = scaling switch {
      InputScaling.QUADRATIC => Mathf.Pow(input, 2),
      InputScaling.CUBIC     => Mathf.Pow(input, 3),
      InputScaling.SQRT      => Mathf.Sqrt(input),
      _                      => input
    };

    return Mathf.Clamp(input, minPower, maxPower);
  }

  private float getPercentPower(float input) {
    return (input - minPower) / (maxPower - minPower);
  }
}
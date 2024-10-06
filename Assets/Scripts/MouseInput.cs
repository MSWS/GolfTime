using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput : MonoBehaviour
{
    private enum InputScaling
    {
        LINEAR,
        QUADRATIC,
        CUBIC,
        SQRT,
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

    /**
    * CLASS VARIABLES
    */

    private float initialScale;
    private Color initialColor;

    private Vector2 downPos;
    private Vector3 initialPowerBarPos, initialPlayerPos;

    private bool canceled = false;

    void OnValidate()
    {
        if (powerBar == null)
            throw new System.Exception("power bar not set");
        if (minPower > maxPower)
            throw new System.Exception("min power is greater than max power");
    }

    void Start()
    {
        initialScale = powerBar.localScale.y;
        initialColor = powerBar.GetComponent<Renderer>().material.color;
    }

    // Manages mouse input
    void Update()
    {
        if (!gameObject.GetComponent<Renderer>().enabled)
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            downPos = Input.mousePosition;
            initialPowerBarPos = powerBar.transform.position;
            initialPlayerPos = transform.position;
            return;
        }

        // Allow user to cancel power bar
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            resetPowerBar();
            canceled = true;
            return;
        }

        Vector2 drag = (Vector2)Input.mousePosition - downPos;
        float scale = scaleInput(drag.magnitude);
        if (Input.GetKey(KeyCode.Mouse0) && !canceled)
        {
            powerBar.localScale = new Vector3(powerBar.localScale.x, initialScale + scale, powerBar.localScale.z);
            powerBar.transform.position = transform.position - initialPlayerPos + initialPowerBarPos + powerBar.transform.up * scale;

            // Change color based on % power
            float percentPower = getPercentPower(scale);
            powerBar.GetComponent<Renderer>().material.color = powerColor.Evaluate(percentPower);
            return;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (canceled)
            {
                canceled = false;
                return;
            }
            // Apply force using powerBar's rotation and scale
            Vector3 force = powerBar.transform.up * scale * -1 * powerScale;
            GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            resetPowerBar();
        }
    }

    private void resetPowerBar()
    {
        powerBar.localScale = new Vector3(powerBar.localScale.x, initialScale, powerBar.localScale.z);
        powerBar.transform.position = initialPowerBarPos + (transform.position - initialPlayerPos);
        powerBar.GetComponent<Renderer>().material.color = initialColor;
    }

    private float scaleInput(float input)
    {
        input *= mouseSensitivity;
        switch (scaling)
        {
            case InputScaling.QUADRATIC:
                input = Mathf.Pow(input, 2);
                break;
            case InputScaling.CUBIC:
                input = Mathf.Pow(input, 3);
                break;
            case InputScaling.SQRT:
                input = Mathf.Sqrt(input);
                break;
        }

        return Mathf.Clamp(input, minPower, maxPower);
    }

    private float getPercentPower(float input)
    {
        return (input - minPower) / (maxPower - minPower);
    }
}
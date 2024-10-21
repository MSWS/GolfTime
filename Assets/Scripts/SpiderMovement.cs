using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpiderMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject, golfBall;
    private Renderer targetRenderer;
    private Animator animator;

    private Vector3 lastPosition;

    [SerializeField]
    private float speed = 0.1f;

    [SerializeField]
    private float maxDistance = 10f;

    [SerializeField]
    private bool onBall = false;

    // Start is called before the first frame update
    void Start()
    {
        targetRenderer = targetObject.GetComponent<Renderer>();
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent?.gameObject == golfBall)
            onBall = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.parent?.gameObject == golfBall)
            onBall = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction;
        float distance = Vector3.Distance(transform.position, golfBall.transform.position);
        if (distance > maxDistance)
        {
            teleportToBall();
            return;
        }

        if (targetRenderer.enabled)
            lastPosition = targetObject.transform.position;

        var targetPosition = targetObject.transform.position;
        var spiderPosition = transform.position;
        direction = targetPosition - spiderPosition;

        if (Vector3.Distance(transform.position, lastPosition) > 0.01f)
        {
            var movement = direction.normalized * speed * Time.deltaTime;
            transform.position += movement;
            animator.SetFloat("Speed", movement.magnitude * 2);
        }

        Quaternion rotation;
        rotation = Quaternion.LookRotation(-direction);
        if (onBall)
        {
            var vecUp = spiderPosition - golfBall.transform.position;
            rotation = Quaternion.LookRotation(-direction, vecUp);
            Debug.DrawRay(spiderPosition, vecUp, Color.green);
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.1f);
        Debug.DrawRay(spiderPosition, direction, Color.red);
    }

    private void teleportToBall()
    {
        var direction = transform.position - golfBall.transform.position;
        transform.position = golfBall.transform.position + direction.normalized * maxDistance;
        Debug.DrawLine(golfBall.transform.position, transform.position, Color.white);
    }
}

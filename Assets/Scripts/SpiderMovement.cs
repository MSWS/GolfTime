using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        targetRenderer = targetObject.GetComponent<Renderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction;
        float distance = Vector3.Distance(transform.position, golfBall.transform.position);
        if (distance > maxDistance)
        {
            direction = transform.position - golfBall.transform.position;
            transform.position = golfBall.transform.position + direction.normalized * maxDistance;
            animator.SetFloat("Speed", 0);
            return;
        }

        if (targetRenderer.enabled)
            lastPosition = targetObject.transform.position;

        var targetPosition = targetObject.transform.position;
        var spiderPosition = transform.position;
        direction = targetPosition - spiderPosition;

        if (Vector3.Distance(transform.position, lastPosition) > 0.1f)
        {
            var movement = direction.normalized * speed * Time.deltaTime;
            transform.position += movement;
        }

        animator.SetFloat("Speed", distance);

        bool isOnBall = distance < 1.1;

        Quaternion rotation;
        rotation = Quaternion.LookRotation(-direction);
        if (isOnBall || true)
        {
            var vecUp = spiderPosition - golfBall.transform.position;
            transform.rotation = Quaternion.LookRotation(-direction, vecUp);
            Debug.DrawRay(spiderPosition, vecUp, Color.green);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.1f);
        }

        Debug.DrawRay(spiderPosition, direction, Color.red);
    }
}

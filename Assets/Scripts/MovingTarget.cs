using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    public Vector3 pointA;                // VR headset initial position
    public Vector3 pointB;                // 80% down position
    public Vector3 pointC;                // 20% up position
    public float initialAcceleration = 0.5f; // Initial acceleration
    public float maxAcceleration = 1f;    // Maximum acceleration
    public float accelerationChangeRate = 0.05f; // Rate at which acceleration increases

    private float currentAcceleration;    // Current acceleration value
    private bool movingDown = true;       // Direction of movement (down or up)
    private Vector3 targetPosition;       // Target position to move toward

    private void Start()
    {
        // Set the initial acceleration
        currentAcceleration = initialAcceleration;

        // Set pointA to the VR headset's initial position
        if (Camera.main != null)
        {
            pointA = Camera.main.transform.position;
        }
        else
        {
            Debug.LogWarning("Main camera not found. Make sure the VR headset is set as the main camera.");
            pointA = transform.position;  // Fallback to current position if camera is not found
        }

        // Calculate pointB as 80% down from pointA on the Y-axis
        pointB = pointA - new Vector3(0f, pointA.y * 0.8f, 0f);

        // Calculate pointC as 20% up from pointA on the Y-axis
        pointC = pointA + new Vector3(0f, pointA.y * 0.2f, 0f);

        // Set the initial target position to pointB (move downward first)
        targetPosition = pointB;

        // Set initial position of the object to the VR headset’s initial position
        transform.position = pointA;
    }

    private void Update()
    {
        // Update acceleration over time
        currentAcceleration = Mathf.Min(currentAcceleration + accelerationChangeRate * Time.deltaTime, maxAcceleration);

        // Move the target object toward the current target position
        MoveTarget();

        // If the target has reached the destination, switch direction
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SwitchDirection();
        }
    }

    private void MoveTarget()
    {
        // Calculate step size based on acceleration and time
        float step = currentAcceleration * Time.deltaTime;

        // Move the target toward the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }

    private void SwitchDirection()
    {
        // Toggle between moving down and up
        if (movingDown)
        {
            targetPosition = pointC;  // Set target to 20% up position
        }
        else
        {
            targetPosition = pointB;  // Set target to 80% down position
        }

        movingDown = !movingDown;  // Switch direction
    }
}

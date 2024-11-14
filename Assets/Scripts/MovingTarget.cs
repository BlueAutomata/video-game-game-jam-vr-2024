using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    public Vector3 pointA;                // First point (lowest position)
    public Vector3 pointB;                // Second point (highest position)
    public float initialAcceleration = 2f; // Initial acceleration
    public float maxAcceleration = 5f;    // Maximum acceleration
    public float accelerationChangeRate = 0.1f; // Rate at which acceleration increases
    public float moveTime = 2f;           // Time to move from one point to another at the current speed

    private float currentAcceleration;    // Current acceleration value
    private bool movingUp = true;         // Direction of movement (up or down)
    private Vector3 targetPosition;       // Target position to move toward

    private void Start()
    {
        // Set the initial acceleration
        currentAcceleration = initialAcceleration;

        // Set pointA as the current position of the object
        pointA = transform.position;

        // Set pointB to be 10 units lower than pointA on the Y-axis
        pointB = pointA - new Vector3(0f, 10f, 0f);

        // Set the initial target position (start at pointB)
        targetPosition = pointB;
    }

    private void Update()
    {
        // Update acceleration over time
        currentAcceleration = Mathf.Min(currentAcceleration + accelerationChangeRate * Time.deltaTime, maxAcceleration);

        // Move the target object toward the current target position
        MoveTarget();

        // If the target has reached the destination, switch to the other point
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
        // Toggle between moving up and down
        if (movingUp)
        {
            targetPosition = pointA;
        }
        else
        {
            targetPosition = pointB;
        }

        movingUp = !movingUp;  // Switch direction
    }
}

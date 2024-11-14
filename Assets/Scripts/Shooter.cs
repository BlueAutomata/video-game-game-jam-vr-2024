using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Shooter : MonoBehaviour
{
    public List<GameObject> projectilePrefabs;  // List of projectile prefabs with different probabilities
    public Transform target;                    // The target object to hit
    public float launchForce = 15f;             // Force applied to each projectile
    public float spreadAngle = 10f;             // Spread angle between projectiles
    public int lineSegmentCount = 50;           // Number of segments in the trajectory line
    public float launchInterval = 1f;           // Interval between each projectile launch
    public float projectileHeightOffset = 0f;   // Height above the shooter to spawn projectiles
    public float lineWidth = 0.05f;             // Width of the trajectory line
    public float acceleration = 2f;             // Acceleration applied to each projectile over time
    public float startTime = 2f;                // Time (in seconds) to wait before starting shooting

    private LineRenderer lineRenderer;

    private void Start()
    {
        // Initialize LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();

        // Set line width
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // Start the coroutine to shoot projectiles after the specified start time
        StartCoroutine(ShootProjectiles());
    }

    private void Update()
    {
        // Draw the trajectory of the projectile
        DrawTrajectory();
    }

    IEnumerator ShootProjectiles()
    {
        int i = 0;

        // Wait until the startTime has passed
        yield return new WaitForSeconds(startTime);

        while (true)  // Infinite loop for shooting projectiles
        {
            // Select a projectile based on probabilities
            GameObject selectedProjectilePrefab = SelectProjectileByProbability();

            // Calculate the spawn position above the shooter
            Vector3 spawnPosition = transform.position + Vector3.up * projectileHeightOffset;

            // Create a projectile instance at the calculated position
            GameObject projectile = Instantiate(selectedProjectilePrefab, spawnPosition, Quaternion.identity);

            // Calculate spread angle for each projectile
            //float angleOffset = (i % 5 - 2) * spreadAngle;  // Spread every 5 projectiles
            //Quaternion spreadRotation = Quaternion.Euler(0, angleOffset, 0);

            // Get direction toward the target and apply spread
            Vector3 direction = (target.position - transform.position).normalized;
            // Vector3 finalDirection = spreadRotation * direction;
            Vector3 finalDirection =  direction;

            // Apply initial force to the projectile's Rigidbody
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.AddForce(finalDirection * launchForce, ForceMode.Impulse);

            // Add continuous acceleration
            StartCoroutine(ApplyAcceleration(rb, finalDirection));

            // Wait for the specified interval before launching the next projectile
            yield return new WaitForSeconds(launchInterval);

            i++;  // Increment to change the spread angle every 5 projectiles
        }
    }

    // Method to select a projectile based on launch probability
    private GameObject SelectProjectileByProbability()
    {
        float totalProbability = 0f;

        // Calculate total probability from all projectiles
        foreach (var prefab in projectilePrefabs)
        {
            ProjectileSettings settings = prefab.GetComponent<ProjectileSettings>();
            if (settings != null)
            {
                totalProbability += settings.launchProbability;
            }
        }

        // Select a random value within the total probability range
        float randomValue = Random.Range(0, totalProbability);
        float cumulativeProbability = 0f;

        // Iterate through projectiles to find the selected one
        foreach (var prefab in projectilePrefabs)
        {
            ProjectileSettings settings = prefab.GetComponent<ProjectileSettings>();
            if (settings != null)
            {
                cumulativeProbability += settings.launchProbability;
                if (randomValue <= cumulativeProbability)
                {
                    return prefab;
                }
            }
        }

        // Fallback in case something goes wrong, return the first projectile
        return projectilePrefabs[0];
    }

    // Coroutine to apply acceleration over time
    IEnumerator ApplyAcceleration(Rigidbody rb, Vector3 direction)
    {
        while (rb != null)  // Continue applying while the projectile exists
        {
            rb.AddForce(direction * acceleration * Time.deltaTime, ForceMode.Acceleration);
            yield return null;  // Apply force every frame
        }
    }

    void DrawTrajectory()
    {
        // Use the center of the shooter prefab as the start position
        Vector3 startPosition = transform.position;
        Vector3 targetDirection = (target.position - startPosition).normalized;

        // Calculate initial velocity vector towards the target
        Vector3 velocity = CalculateLaunchVelocity(startPosition, target.position);

        lineRenderer.positionCount = lineSegmentCount;
        for (int i = 0; i < lineSegmentCount; i++)
        {
            float time = i * 0.1f;  // Step forward in time

            // Calculate point on the trajectory path, considering gravity and acceleration
            Vector3 position = startPosition + velocity * time + 0.5f * Physics.gravity * time * time;

            // Set the position for this segment of the trajectory
            lineRenderer.SetPosition(i, position);

            // Stop if we have reached the target (within a threshold)
            if ((position - target.position).sqrMagnitude < 0.1f * 0.1f) break;
        }

        // Update the shooter's rotation to face the target and align the trajectory direction
        RotateShooterToTrajectory(targetDirection);
    }

    private Vector3 CalculateLaunchVelocity(Vector3 startPosition, Vector3 targetPosition)
    {
        // Get the horizontal distance between start and target
        Vector3 displacement = targetPosition - startPosition;
        float horizontalDistance = new Vector3(displacement.x, 0, displacement.z).magnitude;

        // The angle required to reach the target (assuming 45-degree launch angle for simplicity)
        float gravity = Physics.gravity.y;
        float launchAngle = 45f; // Adjust this if needed for specific scenarios

        // Calculate the initial velocity needed to hit the target (ignoring air resistance)
        float velocityY = Mathf.Sqrt(-2 * gravity * projectileHeightOffset); // Vertical component of velocity
        float velocityX = horizontalDistance / Mathf.Cos(Mathf.Deg2Rad * launchAngle);

        // Combine the velocity components
        Vector3 velocity = (displacement.normalized * velocityX) + (Vector3.up * velocityY);

        return velocity;
    }



    private void RotateShooterToTrajectory(Vector3 trajectoryDirection)
    {
        // Calculate the rotation for the shooter based on the trajectory direction
        Quaternion targetRotation = Quaternion.LookRotation(trajectoryDirection);

        // Apply a 90-degree rotation to the target rotation
        Quaternion rotatedRotation = targetRotation * Quaternion.Euler(90, 0, 0);  // Add 90-degree yaw (rotation around Y-axis)

        // Set the rotation of the shooter to smoothly align with the trajectory direction
        transform.rotation = Quaternion.Slerp(transform.rotation, rotatedRotation, Time.deltaTime * 5f); // Smooth rotation
    }
}

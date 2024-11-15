using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterManager : MonoBehaviour
{
    public List<GameObject> shooterPrefabs;   // List of shooter prefabs to spawn
    public GameObject targetPrefab;           // The target prefab to assign to each shooter
    public int initialShooters = 1;           // Starting number of shooters
    public int maxShooters = 5;               // Maximum number of shooters
    public float spawnInterval = 2f;          // Interval for adding or removing shooters
    public float minShootRate = 0.5f;         // Minimum shoot rate (interval between shots)
    public float maxShootRate = 2f;           // Maximum shoot rate (interval between shots)
    public float shooterMinLifetime = 60f;    // Minimum lifetime for each shooter (1 minute)

    [Header("Shooter Settings")]
    public float launchForce = 15f;           // Base launch force
    public float spreadAngle = 10f;           // Base spread angle
    public float acceleration = 100f;         // Base acceleration
    public float projectileHeightOffset = 1f; // Height offset for projectiles
    public float spawnRadius = 20f;           // Radius for shooter spawn positions

    private List<GameObject> activeShooters = new List<GameObject>();  // List to track active shooters
    private Dictionary<GameObject, float> shooterTimers = new Dictionary<GameObject, float>(); // Track shooter lifetimes
    private int currentShooterCount;           // Current number of shooters

    private void Start()
    {
        // Set the initial shooter count and start the random appearance coroutine
        currentShooterCount = initialShooters;
        StartCoroutine(RandomShooterCoroutine());
    }

    private void ApplyShooterSettings(Shooter shooter)
    {
        // Apply random shooter settings, including a unique shooting rate for each shooter
        shooter.launchForce = launchForce;
        shooter.spreadAngle = spreadAngle;
        shooter.acceleration = acceleration;
        shooter.projectileHeightOffset = projectileHeightOffset;

        // Assign a random shooting rate within the specified range
        shooter.launchInterval = Random.Range(minShootRate, maxShootRate);
    }

    //private List<float> availableAngles = new List<float> { 0f, 72f, 144f, 216f, 288f }; // List of angles available for spawning shooters

    private List<float> availableAngles = new List<float> { 0f, 72f, 144f, 216f, 288f }; // List of angles available for spawning shooters

    private void SpawnShooter()
    {
        // Check if there are any available angles left
        if (availableAngles.Count == 0)
        {
            Debug.LogWarning("No available angles to spawn shooters.");
            return; // No available angles, exit the function
        }

        // Randomly select an angle from the available angles list
        float angle = availableAngles[Random.Range(0, availableAngles.Count)];

        // Remove the selected angle from the list to prevent reuse
        availableAngles.Remove(angle);

        float distance = spawnRadius;

        // Calculate the spawn position using the angle and distance, set y to 0 (floor level)
        Vector3 spawnPosition = targetPrefab.transform.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * distance, -targetPrefab.transform.position.y + 0.4f, Mathf.Sin(angle * Mathf.Deg2Rad) * distance);

        // Instantiate a random shooter prefab at the calculated position
        GameObject shooterObject = Instantiate(shooterPrefabs[Random.Range(0, shooterPrefabs.Count)], spawnPosition, Quaternion.identity);

        // Assign the target prefab to the shooter
        Shooter shooter = shooterObject.GetComponent<Shooter>();
        if (shooter != null)
        {
            shooter.target = targetPrefab.transform; // Assign the target prefab
            ApplyShooterSettings(shooter);
        }

        // Add the shooter to the list of active shooters and set its timer
        activeShooters.Add(shooterObject);
        shooterTimers[shooterObject] = 0f; // Initialize its lifetime timer
        currentShooterCount++;
    }




    private void RemoveRandomShooter()
    {
        // Filter shooters that have been active for at least the minimum lifetime
        List<GameObject> removableShooters = activeShooters.FindAll(s => shooterTimers[s] >= shooterMinLifetime);

        if (removableShooters.Count > 0)
        {
            // Choose a random shooter from those that met the minimum lifetime
            GameObject shooterToRemove = removableShooters[Random.Range(0, removableShooters.Count)];
            activeShooters.Remove(shooterToRemove);
            shooterTimers.Remove(shooterToRemove);

            // Destroy the shooter game object
            Destroy(shooterToRemove);
            currentShooterCount--;
        }
    }

    private IEnumerator RandomShooterCoroutine()
    {
        while (true)
        {
            // Wait for the specified interval before adding/removing shooters
            yield return new WaitForSeconds(spawnInterval);

            // Update lifetime timers for each active shooter
            foreach (var shooter in new List<GameObject>(activeShooters))
            {
                shooterTimers[shooter] += spawnInterval;
            }

            // Randomly decide whether to add or remove a shooter, within limits
            if (currentShooterCount < maxShooters && (currentShooterCount <= initialShooters || Random.value > 0.5f))
            {
                SpawnShooter(); // Add a shooter
            }
            else if (currentShooterCount > initialShooters)
            {
                RemoveRandomShooter(); // Remove a shooter
            }
        }
    }
}

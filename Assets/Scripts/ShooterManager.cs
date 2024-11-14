using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterManager : MonoBehaviour
{
    public List<GameObject> shooterPrefabs;   // List of shooter prefabs to spawn
    public GameObject targetPrefab;            // The target prefab to assign to each shooter
    public int shootersPerLevel = 3;           // Number of shooters to spawn each level
    public int currentLevel = 1;               // Current level of the game (1, 2, or 3)
    public float levelUpInterval = 30f;        // Interval to automatically progress levels

    [Header("Level Settings")]
    public float[] launchForceRange = { 10f, 20f, 30f };  // Launch forces range for each level
    public float[] spreadAngleRange = { 5f, 15f, 25f };   // Spread angle range for each level
    public float[] launchIntervalRange = { 0.5f, 1f, 1.5f }; // Launch interval range for each level
    public float[] accelerationRange = { 3f, 6f, 10f };  // Acceleration range for each level
    public float[] startTimeRange = { 1f, 2f, 3f };      // Start time range for each level
    public float[] projectileHeightOffsetRange = { 0f, 1f, 2f }; // Height offset range for each level
    public float[] radiusRange = { 3f, 4f, 5f };    // Radius range for shooter spawn distances

    private List<GameObject> activeShooters = new List<GameObject>();  // List to track active shooters

    private void Start()
    {
        // Start the coroutine to handle automatic level progression
        StartCoroutine(LevelUpCoroutine());
    }

    private void ApplyShooterSettings(Shooter shooter)
    {
        // Randomly apply the shooter settings based on the current level's range
        shooter.launchForce = Random.Range(launchForceRange[currentLevel - 1] - 5f, launchForceRange[currentLevel - 1] + 5f);
        shooter.spreadAngle = Random.Range(spreadAngleRange[currentLevel - 1] - 5f, spreadAngleRange[currentLevel - 1] + 5f);
        shooter.launchInterval = Random.Range(launchIntervalRange[currentLevel - 1] - 0.2f, launchIntervalRange[currentLevel - 1] + 0.2f);
        shooter.acceleration = Random.Range(accelerationRange[currentLevel - 1] - 2f, accelerationRange[currentLevel - 1] + 2f);
        shooter.startTime = Random.Range(startTimeRange[currentLevel - 1] - 0.5f, startTimeRange[currentLevel - 1] + 0.5f);
        shooter.projectileHeightOffset = Random.Range(projectileHeightOffsetRange[currentLevel - 1] - 0.5f, projectileHeightOffsetRange[currentLevel - 1] + 0.5f);
    }

    private void SpawnShooters()
    {
        // Calculate the spawn radius for this level
        float spawnRadius = 20; //radiusRange[currentLevel - 1];

        // Spawn shooters in a circle around the target
        for (int i = 0; i < shootersPerLevel; i++)
        {
            // Calculate a random position within the radius
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(0f, spawnRadius);

            Vector3 spawnPosition = targetPrefab.transform.position + new Vector3(Mathf.Cos(angle) * distance, 0, Mathf.Sin(angle) * distance);

            // Set the Y position to the target's Y position to ensure they spawn at the correct height
            spawnPosition.y = targetPrefab.transform.position.y;

            // Instantiate the shooter prefab at the calculated position
            GameObject shooterObject = Instantiate(shooterPrefabs[Random.Range(0, shooterPrefabs.Count)], spawnPosition, Quaternion.identity);

            // Assign the target prefab to the shooter
            Shooter shooter = shooterObject.GetComponent<Shooter>();
            if (shooter != null)
            {
                shooter.target = targetPrefab.transform; // Assign the target prefab
                ApplyShooterSettings(shooter);
            }

            // Add the shooter to the list of active shooters
            activeShooters.Add(shooterObject);
        }
    }

    private void DestroyPreviousShooters()
    {
        // Destroy all shooters from the previous level
        foreach (GameObject shooter in activeShooters)
        {
            Destroy(shooter);
        }

        // Clear the list of active shooters for the new level
        activeShooters.Clear();
    }

    private IEnumerator LevelUpCoroutine()
    {
        // Wait for the specified interval before progressing to the next level
        while (true)
        {
            yield return new WaitForSeconds(levelUpInterval);

            // Destroy shooters from the previous level
            DestroyPreviousShooters();

            // Increase level (loop back to level 1 after level 3)
            currentLevel = (currentLevel % 3) + 1;

            // Spawn shooters for the new level with randomized settings
            SpawnShooters();
        }
    }
}

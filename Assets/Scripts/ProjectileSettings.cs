using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSettings : MonoBehaviour
{
    [Range(0f, 1f)]
    public float launchProbability = 0.5f;  // Probability to be launched (0 to 1)

    // Enum to define the type of the projectile
    public enum ProjectileType
    {
        Attack,   // For attacking projectiles
        Defense,  // For defensive projectiles
        Health,   // For health-related projectiles
        Armor,    // For armor-related projectiles
        Danger    // For danger projectiles
    }

    // Public variable to assign the type in the Inspector
    public ProjectileType projectileType;

    // Reference to the Player script (drag Player object into this in the Inspector)
    public Player player;

    private void Update()
    {
        // Check if the projectile is below the ground (negative y-axis value)
        if (transform.position.y < 0f)
        {
            DestroyProjectile();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Handle collisions with different types of objects
        if (collision.gameObject.CompareTag("Bullet"))
        {
            HandleBulletCollision(collision); // Handle Bullet collision separately
        }
        else if (collision.gameObject.CompareTag("Shield"))
        {
            HandleShieldCollision(collision); // Handle Shield collision separately
        }

        // Handle specific projectile type actions when colliding
        HandleProjectileTypeCollision(collision);
    }

    // Method to handle different projectile types upon collision
    private void HandleProjectileTypeCollision(Collision collision)
    {
        switch (projectileType)
        {
            case ProjectileType.Attack:
                // Handle behavior for Attack projectiles
                Debug.Log("Attack projectile collided!");
                break;

            case ProjectileType.Defense:
                // Handle behavior for Defense projectiles
                Debug.Log("Defense projectile collided!");
                break;

            case ProjectileType.Health:
                // Handle behavior for Health projectiles
                Debug.Log("Health projectile collided!");
                break;

            case ProjectileType.Armor:
                // Handle behavior for Armor projectiles
                Debug.Log("Armor projectile collided!");
                break;

            case ProjectileType.Danger:
                // Handle behavior for Danger projectiles
                Debug.Log("Danger projectile collided! Apply damage or debuff!");
                ApplyDangerEffect(collision);  // Apply custom danger effect (e.g., damage or debuff)
                break;
        }

        // Destroy the projectile after the collision
        DestroyProjectile();
    }

    // Method to apply danger effect on collision
    private void ApplyDangerEffect(Collision collision)
    {
        // Check if the collided object is the player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Handle behavior for Danger projectiles hitting the player
            if (player != null)
            {
                if (collision.gameObject.CompareTag("Bullet"))
                {
                    player.SubtractHealth(10);  // Apply damage to health if hit by bullet
                }
                else if (collision.gameObject.CompareTag("Shield"))
                {
                    player.SubtractArmor(5);  // Apply damage to armor if hit by shield
                }
            }
        }
    }

    // Method to destroy the projectile
    private void DestroyProjectile()
    {
        Destroy(gameObject);  // Destroy this gameObject (the projectile clone)
    }

    // Method to handle collision with Bullet
    private void HandleBulletCollision(Collision collision)
    {
        // Additional logic for Bullet collision can be added here
        Debug.Log("Projectile collided with Bullet!");
        DestroyProjectile(); // Destroy the projectile when it hits Bullet
    }

    // Method to handle collision with Shield
    private void HandleShieldCollision(Collision collision)
    {
        // Additional logic for Shield collision can be added here
        Debug.Log("Projectile collided with Shield!");
        DestroyProjectile(); // Destroy the projectile when it hits Shield
    }
}


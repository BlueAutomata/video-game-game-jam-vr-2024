using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health = 100;  // Player's health points
    public int armor = 50;    // Player's armor points

    // Method to subtract health
    public void SubtractHealth(int amount)
    {
        health -= amount;
        if (health < 0) health = 0;  // Prevent negative health
        Debug.Log("Health: " + health);
    }

    // Method to subtract armor
    public void SubtractArmor(int amount)
    {
        armor -= amount;
        if (armor < 0) armor = 0;  // Prevent negative armor
        Debug.Log("Armor: " + armor);
    }
}
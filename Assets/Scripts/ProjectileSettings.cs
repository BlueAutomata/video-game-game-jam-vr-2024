using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSettings : MonoBehaviour
{
    [Range(0f, 1f)]
    public float launchProbability = 0.5f;  // Probability to be launched (0 to 1)
}

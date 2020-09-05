using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : EntityHealth
{
    [Header("Player Health")]
    public int segmentCount = 7;

    [Tooltip("How much the health decreases a segment each seconds")]
    public float segmentReduceRate = .5f;

    private void Update()
    {
        Health -= segmentReduceRate / segmentCount * Time.deltaTime;
    }

    /// <summary> Increase the player health amount by a segment </summary>
    /// <param name="segmentCount">The amount of segment to be added to the player's health</param>
    public void IncreaseHealth(int segmentCount)
    {
        Health += 1f / segmentCount;
    }
}

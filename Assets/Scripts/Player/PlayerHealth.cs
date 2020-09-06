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
        Health = Mathf.Clamp01(Health);
    }

	/// <summary> Increase the player health amount by n segment </summary>
	/// <param name="segment">The amount of segment to be added to the player's health</param>
	public void IncreaseHealth(int segment)
	{
		Health = Mathf.Min(1f, 1f / segmentCount * segment);
	}
}

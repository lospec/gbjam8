using System.Collections;
using UnityEngine;

public class RisingLava : MonoBehaviour
{
	[SerializeField] private float _risingSpeed;
	[SerializeField] private float _delayBeforeDeathSec;

    private void Update()
    {
		transform.position = new Vector3(
			gameObject.transform.position.x,
			gameObject.transform.position.y + _risingSpeed * Time.deltaTime,
			gameObject.transform.position.z);
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
		if (playerHealth != null)
		{
			StartCoroutine(WaitThenKillPlayer(playerHealth));
		}
	}

	private IEnumerator WaitThenKillPlayer(PlayerHealth playerHealth)
	{
		yield return new WaitForSeconds(_delayBeforeDeathSec);

		playerHealth.Die();
	}
}

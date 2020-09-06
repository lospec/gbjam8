using System.Collections;
using UnityEngine;

public class RisingLava : MonoBehaviour
{
    [SerializeField] private float _risingSpeed;
    [SerializeField] private GameObject explosion;
	[SerializeField] private float _delayBeforeDeathSec;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _increaseSpeed;

	private void Start()
    {
        GetComponent<MeshRenderer>().sortingOrder = 2;
    }

    private void Update()
    {
		transform.position = new Vector3(
			gameObject.transform.position.x,
			gameObject.transform.position.y + _risingSpeed * Time.deltaTime,
			gameObject.transform.position.z);

        _risingSpeed += Time.deltaTime * _increaseSpeed;
        _risingSpeed = Mathf.Clamp(_risingSpeed, 0, _maxSpeed);
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
		if (playerHealth != null)
		{
			Instantiate(explosion, collision.transform.position, Quaternion.identity);
			StartCoroutine(WaitThenKillPlayer(playerHealth));
		}
	}

	private IEnumerator WaitThenKillPlayer(PlayerHealth playerHealth)
	{
        StartCoroutine(TransitionManager.Instance.DoubleTransition());

        yield return new WaitForSeconds(_delayBeforeDeathSec);

		playerHealth.Die();
	}
}

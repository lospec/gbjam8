using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform _target;
	[SerializeField] private float _smoothMovementWeight = 0.01f;
	[SerializeField] private float _minDistForMovement = 0;
	[SerializeField] private float _minPosY = 0;

    void Update()
    {
		if (Mathf.Abs(_target.position.y - transform.position.y) > _minDistForMovement)
		{
			transform.position = Vector3.Lerp(transform.position,
				new Vector3(
				transform.position.x,
				Mathf.Max(_target.position.y, _minPosY),
				transform.position.z),
				_smoothMovementWeight);
		}
    }
}

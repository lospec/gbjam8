using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private Transform _target;
	[SerializeField] private float _smoothMovementMaxSpeed = Mathf.Infinity;
	[SerializeField] private float _minPosY = Mathf.NegativeInfinity;
	[SerializeField] private float _maxPosY = Mathf.Infinity;
	[SerializeField] private float _targetOffsetY = 0;

	private Vector3 _velocity;

	private void LateUpdate()
	{
		Vector3 targetPos = new Vector3(transform.position.x, _target.position.y + _targetOffsetY, transform.position.z);
		targetPos = new Vector3(targetPos.x, Mathf.Clamp(targetPos.y, _minPosY, _maxPosY), targetPos.z);

		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, default, _smoothMovementMaxSpeed);
	}
}

using UnityEngine;

public class ParallaxBackgroundController : MonoBehaviour
{
	[SerializeField] private Transform _background1;
	[SerializeField] private Transform _background2;

	private float _size;

	private void Start()
	{
		_size = _background1.GetComponent<BoxCollider2D>().size.y;
	}

	private void Update()
	{
		if (transform.position.y > _background2.position.y)
		{
			_background1.position = new Vector3(_background1.position.x, _background2.position.y + _size, _background1.position.z);
			SwitchBackgrounds();
		}

		if (transform.position.y < _background1.position.y)
		{
			_background2.position = new Vector3(_background2.position.x, _background1.position.y - _size, _background2.position.z);
			SwitchBackgrounds();
		}
	}

	private void SwitchBackgrounds()
	{
		Transform tempBackground1 = _background1;
		_background1 = _background2;
		_background2 = tempBackground1;
	}
}
using UnityEngine;
using UnityEngine.UI;

public class HUDProgressBar : MonoBehaviour
{
	[SerializeField] private Player.CharacterController _player;
	[SerializeField] private float _minPlayerPosY;
	[SerializeField] private float _maxPlayerPosY;
	[SerializeField] private Slider _slider;

	private void Update()
	{
		float value = Mathf.InverseLerp(_minPlayerPosY, _maxPlayerPosY, _player.transform.position.y);
		_slider.value = value;
	}
}

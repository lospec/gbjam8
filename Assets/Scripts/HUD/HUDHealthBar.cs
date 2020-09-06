using UnityEngine;
using UnityEngine.UI;

public class HUDHealthBar : MonoBehaviour
{
	[SerializeField] private Slider _slider;
	[SerializeField] private PlayerHealth _playerHealth;

	public void SetHUDHealth(float health)
	{
		_slider.value = _playerHealth.Health;
	}
}

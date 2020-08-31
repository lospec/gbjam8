using UnityEngine;

public class HUDHealthHandler : MonoBehaviour
{
	[SerializeField] private EntityHealth _targetEntityHealth;
	[SerializeField] private GameObject _healthImagePrefab;
	[SerializeField] private int _healthImagePadding;
	[SerializeField] private int _healthImageTopPadding;

    private void Start()
    {
		_targetEntityHealth.HealthSetEvent += SetHUDHealth;
	}

	private void SetHUDHealth(int health)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		int numHealthImages = health - transform.childCount;

		for (int i = 0; i < numHealthImages; i++)
		{
			GameObject heartImage = Instantiate(_healthImagePrefab);
			heartImage.transform.SetParent(transform);

			RectTransform heartImageRectTransform = heartImage.GetComponent<RectTransform>();
			heartImageRectTransform.anchoredPosition = new Vector3(
				-heartImageRectTransform.rect.width / 2,
				-heartImageRectTransform.rect.height / 2 - _healthImageTopPadding - (heartImageRectTransform.rect.height + _healthImagePadding) * i,
				0);
			heartImageRectTransform.localScale = Vector3.one;
		}
	}
}

using UnityEngine;

public class HUDCounterHandler : MonoBehaviour
{
	[SerializeField] private GameObject _imagePrefab;
	[SerializeField] private int _imagePadding;
	[SerializeField] private int _imageTopPadding;

	public void SetHUDCounter(int value)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		int numImages = value - transform.childCount;

		for (int i = 0; i < numImages; i++)
		{
			GameObject image = Instantiate(_imagePrefab);
			image.transform.SetParent(transform);

			RectTransform imageRectTransform = image.GetComponent<RectTransform>();
			imageRectTransform.anchoredPosition = (Vector3)Vector3Int.RoundToInt(new Vector3(
				-imageRectTransform.rect.width / 2,
				-imageRectTransform.rect.height / 2 - _imageTopPadding - (imageRectTransform.rect.height + _imagePadding) * i,
				0));
			imageRectTransform.localScale = Vector3.one;
		}
	}
}

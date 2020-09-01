using TMPro;
using UnityEngine;

public class HUDScoreHandler : MonoBehaviour
{
	[SerializeField] private TMP_Text _textMeshProText;

	public void SetHUDScore(int score)
	{
		_textMeshProText.text = string.Join("\n", score.ToString().ToCharArray());
	}
}

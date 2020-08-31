using System.Linq;
using TMPro;
using UnityEngine;

public class HUDScoreHandler : MonoBehaviour
{
	[SerializeField] private EntityScore _targetEntityScore;
	[SerializeField] private TMP_Text _textMeshProText;

	private void Awake()
	{
		_targetEntityScore.ScoreSetEvent += SetHUDScore;
	}

	private void SetHUDScore(int score)
	{
		_textMeshProText.text = string.Join("\n", score.ToString().ToArray());
	}
}

using TMPro;
using UnityEngine;

public class HUDTimer : MonoBehaviour
{
	[SerializeField] private TMP_Text _textMeshProText;

	private int totalSecondsPassed;
	private float secondsPassedSinceLastSecond = 0;

	private void Update()
	{
		secondsPassedSinceLastSecond += Time.deltaTime;
		if (secondsPassedSinceLastSecond >= 1)
		{
			secondsPassedSinceLastSecond = 0;
			totalSecondsPassed++;
			UpdateHUDTimer();
		}
	}

	private void Start()
	{
		UpdateHUDTimer();
	}

	private void UpdateHUDTimer()
	{
		int secondsPassed = totalSecondsPassed % 60;
		int minutesPassed = totalSecondsPassed / 60;
		string secondsFormatted = secondsPassed.ToString().PadLeft(2, '0');
		string minutesFormatted = minutesPassed.ToString().PadLeft(2, '0');
		string timeFormatted = minutesFormatted + '-' + secondsFormatted;
		_textMeshProText.text = string.Join("\n", timeFormatted.ToCharArray());
	}
}

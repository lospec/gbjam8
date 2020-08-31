using UnityEngine;

class EntityScore : MonoBehaviour
{
	[SerializeField] private int _score;
	public int Score
	{
		protected set
		{
			_score = value;
			OnScoreSet(value);
		}
		get
		{
			return _score;
		}
	}

	public delegate void ScoreSetEventHandler(int score);
	public event ScoreSetEventHandler ScoreSetEvent;

	protected virtual void OnScoreSet(int health)
	{
		ScoreSetEvent?.Invoke(health);
	}

	private void Start()
	{
		OnScoreSet(Score);
	}
}

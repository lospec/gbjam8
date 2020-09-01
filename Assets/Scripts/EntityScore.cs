using UnityEngine;
using UnityEngine.Events;

class EntityScore : MonoBehaviour
{
	[SerializeField] private int _score;
	public int Score
	{
		protected set
		{
			_score = value;
			OnScoreSet?.Invoke(value);
		}
		get
		{
			return _score;
		}
	}

	[SerializeField] public UnityEvent<int> OnScoreSet;

	private void Start()
	{
		OnScoreSet?.Invoke(Score);
	}
}

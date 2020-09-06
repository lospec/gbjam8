using UnityEngine;

public class PersistentAudioSystem : MonoBehaviour
{
	private void Awake()
	{
		GameObject[] audioSystems = GameObject.FindGameObjectsWithTag("PersistentAudioSystem");
		if (audioSystems.Length > 1)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}
}

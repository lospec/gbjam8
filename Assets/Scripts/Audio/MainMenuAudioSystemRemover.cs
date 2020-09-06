using UnityEngine;

public class MainMenuAudioSystemRemover : MonoBehaviour
{
	private void Awake()
	{
		GameObject audioSystem = GameObject.FindGameObjectWithTag("PersistentAudioSystem");
		Destroy(audioSystem);
	}
}

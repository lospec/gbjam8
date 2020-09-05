using Audio;
using UnityEngine;

public class EnemySFXConnectionHandler : MonoBehaviour
{
	[SerializeField] private EntityHealth _entityHealth;
	[SerializeField] private AudioManagerAudioClip _enemyHurt;

	void Start()
    {
		GameObject audioManager = GameObject.Find("AudioSystem");
		AudioManager audioManagerScript = audioManager.GetComponent<AudioManager>();
		_entityHealth.OnTakeDamage.AddListener(c => audioManagerScript.PlayAudio(_enemyHurt));
	}
}

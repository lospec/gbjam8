using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[SerializeField] private List<AudioManagerAudioClip> _audioClips;

	readonly private Dictionary<AudioManagerAudioClip, AudioSource> _audioSystemClipToSource = new Dictionary<AudioManagerAudioClip, AudioSource>();

	private void Start()
	{
		foreach (AudioManagerAudioClip audioSystemAudioClip in _audioClips)
		{
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.clip = audioSystemAudioClip.AudioClip;
			audioSource.volume = audioSystemAudioClip.Volume;
			audioSource.pitch = audioSystemAudioClip.Pitch;

			_audioSystemClipToSource.Add(audioSystemAudioClip, audioSource);
		}
	}

	public void PlayAudio(string name)
	{
		var audioSources =
			from pair in _audioSystemClipToSource
			where pair.Key.Name == name
			select pair.Value;

		int count = audioSources.Count();
		if (count > 1)
		{
			Debug.LogException(new System.Exception(string.Format("{0} audio sources with the same name of {1} were found", count, name)));
			return;
		}
		if (count < 1)
		{
			Debug.LogException(new System.Exception(string.Format("No audio sources with the name of {0} were found", name)));
			return;
		}

		AudioSource audioSource = audioSources.First();
		audioSource.Play();
	}
}

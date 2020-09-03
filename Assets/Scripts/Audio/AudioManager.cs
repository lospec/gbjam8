using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Audio
{
	public class AudioManager : MonoBehaviour
    {
        [FormerlySerializedAs("_audioClips")] [SerializeField]
        private List<AudioManagerAudioClip> audioClips;

		[SerializeField] private List<AudioManagerAudioClip> _startingAudioClips = new List<AudioManagerAudioClip>();

        private readonly Dictionary<AudioManagerAudioClip, AudioSource>
            _audioManagerClipToSource =
                new Dictionary<AudioManagerAudioClip, AudioSource>();

        private void Start()
        {
            foreach (var audioSystemAudioClip in audioClips)
            {
				if (audioSystemAudioClip == null)
				{
					Debug.LogException(new System.Exception(
						"An AudioSystemAudioClip was found null in the Audio Clips list"));
					continue;
				}

                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = audioSystemAudioClip.audioClip;

                _audioManagerClipToSource.Add(audioSystemAudioClip, audioSource);
            }

			if (_startingAudioClips.Count > 0)
			{
				StartCoroutine(PlayStartingAudio());
			}
        }

		private IEnumerator PlayStartingAudio()
		{
			foreach (AudioManagerAudioClip clip in _startingAudioClips)
			{
				PlayAudio(clip);
				yield return new WaitForSeconds(clip.audioClip.length);
			}
		}

        private AudioSource GetClipSource(AudioManagerAudioClip clip)
        {
			return _audioManagerClipToSource[clip];
        }

        public void PlayAudio(AudioManagerAudioClip clip)
        {
            var audioSource = GetClipSource(clip);
            audioSource.volume = clip.volume;
            audioSource.pitch = clip.pitch;
			audioSource.loop = clip.loop;
            audioSource.Play();
        }

        public void IncreaseVolume(AudioManagerAudioClip clip)
        {
			var audioSource = GetClipSource(clip);
			audioSource.volume += clip.volumeIncrementAmount;
		}
    }
}
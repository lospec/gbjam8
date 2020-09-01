using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [FormerlySerializedAs("_audioClips")] [SerializeField]
        private List<AudioManagerAudioClip> audioClips;

        private readonly Dictionary<AudioManagerAudioClip, AudioSource>
            _audioSystemClipToSource =
                new Dictionary<AudioManagerAudioClip, AudioSource>();

        private void Start()
        {
            foreach (var audioSystemAudioClip in audioClips)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = audioSystemAudioClip.audioClip;
                audioSource.volume = audioSystemAudioClip.volume;
                audioSource.pitch = audioSystemAudioClip.pitch;

                _audioSystemClipToSource.Add(audioSystemAudioClip, audioSource);
            }
        }

        public void PlayAudio(string audioName)
        {
            var audioSources =
                (from pair in _audioSystemClipToSource
                    where pair.Key.name == audioName
                    select pair.Value).ToArray();

            var count = audioSources.Length;
            if (count > 1)
            {
                Debug.LogException(new System.Exception(string.Format(
                    "{0} audio sources with the same audioName of {1} were found",
                    count,
                    audioName)));
                return;
            }

            if (count < 1)
            {
                Debug.LogException(new System.Exception(
                    string.Format(
                        "No audio sources with the audioName of {0} were found",
                        audioName)));
                return;
            }

            var audioSource = audioSources.First();
            audioSource.Play();
        }
    }
}
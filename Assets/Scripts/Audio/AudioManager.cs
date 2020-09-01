using System;
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
            _audioManagerClipToSource =
                new Dictionary<AudioManagerAudioClip, AudioSource>();

        private void Start()
        {
            foreach (var audioSystemAudioClip in audioClips)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = audioSystemAudioClip.audioClip;

                _audioManagerClipToSource.Add(audioSystemAudioClip, audioSource);
            }
        }

        private KeyValuePair<AudioManagerAudioClip, AudioSource>
            GetAudioSourcePairByName(string audioName)
        {
            var audioSourcePairs =
                (from pair in _audioManagerClipToSource
                    where pair.Key.name == audioName
                    select pair).ToArray();

            var count = audioSourcePairs.Length;
            if (count > 1)
            {
                Debug.LogException(new System.Exception(string.Format(
                    "{0} audio sources with the same audioName of {1} were found",
                    count,
                    audioName)));
                return new KeyValuePair<AudioManagerAudioClip, AudioSource>();
            }

            if (count < 1)
            {
                Debug.LogException(new System.Exception(
                    string.Format(
                        "No audio sources with the audioName of {0} were found",
                        audioName)));
                return new KeyValuePair<AudioManagerAudioClip, AudioSource>();
            }

            return audioSourcePairs.First();
        }

        private AudioSource GetClipSource(AudioManagerAudioClip clip)
        {
            throw new NotImplementedException();
        }

        public void PlayAudio(AudioManagerAudioClip clip)
        {
            var audioSource = GetClipSource(clip);
            audioSource.volume = clip.volume;
            audioSource.pitch = clip.pitch;
            audioSource.Play();
        }

        public void IncreaseVolume(string audioName)
        {
            KeyValuePair<AudioManagerAudioClip, AudioSource> pair =
                GetAudioSourcePairByName(audioName);
            AudioManagerAudioClip audioManagerAudioClip = pair.Key;
            AudioSource audioSource = pair.Value;
            audioSource.volume += audioManagerAudioClip.VolumeIncrementAmount;
        }
    }
}
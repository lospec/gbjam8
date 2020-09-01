﻿using System.Collections.Generic;
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
                Debug.LogException(new System.Exception(
                    $"{count} audio sources with the same audioName of {audioName} were found"));
                return new KeyValuePair<AudioManagerAudioClip, AudioSource>();
            }

            if (count < 1)
            {
                Debug.LogException(new System.Exception(
                    $"No audio sources with the audioName of {audioName} were found"));
                return new KeyValuePair<AudioManagerAudioClip, AudioSource>();
            }

            return audioSourcePairs.First();
        }

        public void PlayAudio(string audioName)
        {
            KeyValuePair<AudioManagerAudioClip, AudioSource> pair =
                GetAudioSourcePairByName(audioName);
            AudioManagerAudioClip audioManagerAudioClip = pair.Key;
            AudioSource audioSource = pair.Value;
            audioSource.volume = audioManagerAudioClip.volume;
            audioSource.pitch = audioManagerAudioClip.pitch;

            audioSource.Play();
        }

        public void IncreaseVolume(string audioName)
        {
            KeyValuePair<AudioManagerAudioClip, AudioSource> pair =
                GetAudioSourcePairByName(audioName);
            AudioManagerAudioClip audioManagerAudioClip = pair.Key;
            AudioSource audioSource = pair.Value;
            audioSource.volume += audioManagerAudioClip.volumeIncrementAmount;
        }
    }
}
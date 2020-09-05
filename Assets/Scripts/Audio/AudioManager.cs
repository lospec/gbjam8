using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSourcePrefab;
        [SerializeField]
        private List<AudioManagerAudioClip> startingAudioClips =
            new List<AudioManagerAudioClip>();

        [Header("Settings")]
        [SerializeField] private int startingPool = 4;

        private readonly List<AudioSource>
            _pooledAudioSources = new List<AudioSource>();


        private void Start()
        {
            for (var i = 0; i < startingPool; i++)
            {
                CreateAudioSource();
            }

            if (startingAudioClips.Count > 0)
            {
                PlayStartingAudio();
            }
        }

        private void Update()
        {
            _pooledAudioSources.Where(source => source.gameObject.activeSelf).Where
                (source => !source.isPlaying).ToList().ForEach(source => source
                .gameObject.SetActive(false));
        }

        private AudioSource CreateAudioSource(bool active = false)
        {
            var audioSource = Instantiate(audioSourcePrefab, transform, true);
            audioSource.gameObject.SetActive(active);
            _pooledAudioSources.Add(audioSource);
            return audioSource;
        }

        private void PlayStartingAudio()
        {
            AudioSource previousSource = null;
            foreach (var clip in startingAudioClips)
            {
                var source = GetAudioSource(clip);
                if (previousSource != null)
                {
                    source.PlayDelayed(previousSource.clip.length);
                }
                else
                {
                    source.Play();
                }

                previousSource = source;
            }
        }

        private AudioSource GetClipSource(AudioManagerAudioClip clip)
        {
            var audioSource =
                _pooledAudioSources.FirstOrDefault(source =>
                    !source.gameObject.activeSelf);

            if (audioSource == null)
            {
                audioSource = CreateAudioSource(true);
            }
            else
            {
                audioSource.gameObject.SetActive(true);
            }

            audioSource.clip = clip.audioClip;
            return audioSource;
        }

        public AudioSource GetAudioSource(AudioManagerAudioClip clip)
        {
            var audioSource = GetClipSource(clip);
            audioSource.volume = clip.volume;
            audioSource.pitch = clip.pitch;
            audioSource.loop = clip.loop;
            return audioSource;
        }

        public void PlayAudio(AudioManagerAudioClip clip) =>
            GetAudioSource(clip).Play();

        public void IncreaseVolume(AudioManagerAudioClip clip)
        {
            var audioSource = GetClipSource(clip);
            audioSource.volume += clip.volumeIncrementAmount;
        }
    }
}